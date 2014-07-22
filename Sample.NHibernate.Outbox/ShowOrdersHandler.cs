using System;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;

namespace Sample.NHibernate.Outbox
{
    class ShowOrdersHandler : IHandleMessages<ShowOrders>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public void Handle(ShowOrders message)
        {
            Console.Out.WriteLine("Current shipped orders:");

            var orders = NHibernateStorageContext.Session.QueryOver<Entities.Order>()
                .Where(o => o.Shipped)
                .List();

            foreach (var order in orders)
            {
                Console.Out.WriteLine("{0} - {1}", order.Id, order.Product);
            }
        }
    }
}