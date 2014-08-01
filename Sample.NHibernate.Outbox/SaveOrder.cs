using System;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;
using NServiceBus.Saga;
using Order = Sample.NHibernate.Outbox.Entities.Order;

namespace Sample.NHibernate.Outbox
{
    internal class SaveOrder : Saga<SaveOrder.OrderData>, IAmStartedByMessages<NewOrder>,
        IHandleMessages<BuyersRemorseIsOver>,IHandleMessages<OrderIsReadyToBeShipped>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public void Handle(NewOrder message)
        {
            Data.OrderId = message.OrderId;
            Data.Product = message.Product;
            Data.Quantity = message.Quantity;
            Console.Out.WriteLine("New Order {0} has arrived", Data.OrderId);

            Bus.SendLocal(new StartBuyersRemorse
            {
                OrderId = Data.OrderId,
            });
        }

        public void Handle(BuyersRemorseIsOver message)
        {
            Console.Out.WriteLine("Buyers remorse is over for order {0}", Data.OrderId);

            var order = new Order
            {
                Id = Data.OrderId,
                Product = Data.Product,
                Quantity = Data.Quantity
            };

            NHibernateStorageContext.Session.Save(order);

            Bus.Publish(new OrderSubmitted
            {
                OrderId = Data.OrderId,
            });
        }

        public void Handle(OrderIsReadyToBeShipped message)
        {
            Console.Out.WriteLine("{0} order is ready to be shipped", Data.OrderId);

            Bus.SendLocal(new OrderPlaced
            {
                OrderId = Data.OrderId,
            });

            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderData> mapper)
        {
            mapper.ConfigureMapping<NewOrder>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<BuyersRemorseIsOver>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<OrderIsReadyToBeShipped>(m => m.OrderId).ToSaga(s => s.OrderId);
        }

        internal class OrderData : ContainSagaData
        {
            public virtual int Quantity { get; set; }
            public virtual string Product { get; set; }
            
            [Unique]
            public virtual Guid OrderId { get; set; }
        }
    }
}