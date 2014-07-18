using System;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;
using Order = Sample.NHibernate.Outbox.Entities.Order;

namespace Sample.NHibernate.Outbox
{
    internal class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public void Handle(OrderPlaced message)
        {
            Console.Out.WriteLine("Order #{0} being shipped now", message.OrderId);

            var order = NHibernateStorageContext.Session.Get<Entities.Order>(message.OrderId);

            order.Shipped = true;

            NHibernateStorageContext.Session.Update(order);
        }
    }
}