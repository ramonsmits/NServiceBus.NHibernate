using System;
using NServiceBus.Persistence.NHibernate;
using NServiceBus.Saga;
using Sample.NHibernate.Outbox.Entities;

namespace Sample.NHibernate.Outbox
{
    internal class SaveOrder : Saga<SaveOrder.OrderData>, IAmStartedByMessages<NewOrder>,
        IHandleTimeouts<BuyersRemorseIsOver>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public void Handle(NewOrder message)
        {
            Console.Out.WriteLine("Processing order");

            RequestTimeout(TimeSpan.FromSeconds(5), new BuyersRemorseIsOver());

            Data.ThrowException = Bus.CurrentMessageContext.Headers.ContainsKey("ThrowException");
            Data.Product = message.Product;
            Data.Quantity = message.Quantity;
        }

        public void Timeout(BuyersRemorseIsOver state)
        {
            Console.Out.WriteLine("Order fulfilled");

            var order = new Order
            {
                Product = Data.Product,
                Quantity = Data.Quantity
            };

            NHibernateStorageContext.Session.Save(order);

            Bus.Reply(new OrderPlaced
            {
                OrderId = order.Id,
            });

            MarkAsComplete();

            if (Data.ThrowException)
            {
                throw new Exception("Every 3rd order will fail!");
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderData> mapper)
        {
        }

        internal class OrderData : ContainSagaData
        {
            public virtual int Quantity { get; set; }
            public virtual string Product { get; set; }
            public virtual bool ThrowException { get; set; }
        }
    }

    internal class BuyersRemorseIsOver
    {
    }
}