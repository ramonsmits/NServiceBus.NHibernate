using System;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;
using Sample.NHibernate.Outbox;
using Order = Sample.NHibernate.Outbox.Entities.Order;

namespace OutboxSubscriber
{
    class OrderSubmittedHandler : IHandleMessages<OrderSubmitted>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public IBus Bus { get; set; }

        public void Handle(OrderSubmitted message)
        {
            var order = NHibernateStorageContext.Session.Get<Order>(message.OrderId);

            order.Prepared = true;

            NHibernateStorageContext.Session.Save(order);

            Console.Out.WriteLine("Order #{0} is ready to be shipped", message.OrderId);

            Bus.Reply(new OrderIsReadyToBeShipped { OrderId = message.OrderId });
        }
    }
}
