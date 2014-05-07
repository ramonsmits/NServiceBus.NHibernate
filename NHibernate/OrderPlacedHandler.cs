using System;
using NHibernate;
using NServiceBus;

namespace Test.NHibernate
{
    internal class OrderPlacedHandler2 : IHandleMessages<OrderPlaced>
    {
        public ISession Session { get; set; }

        public void Handle(OrderPlaced message)
        {
            Console.Out.WriteLine("Order #{0} being shipped now", message.OrderId);

            var order = Session.Get<Entities.Order>(message.OrderId);

            order.Shipped = true;

            Session.Update(order);
        }
    }

    class OrderPlacedHandler : HandlerWithNHibernateSession<OrderPlaced>
    {
        public override void Handle(OrderPlaced message)
        {
            Console.Out.WriteLine("Order #{0} being shipped now", message.OrderId);

            var order = Session.Get<Entities.Order>(message.OrderId);

            order.Shipped = true;

            Session.Update(order);
        }
    }
}