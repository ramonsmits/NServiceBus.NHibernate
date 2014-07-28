using System;
using System.Threading;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;

namespace Sample.NHibernate.Outbox
{
    class StartBuyersRemorseHandler : IHandleMessages<StartBuyersRemorse>
    {
        public IBus Bus { get; set; }

        public void Handle(StartBuyersRemorse message)
        {
            Bus.Reply(new BuyersRemorseIsOver { OrderId = message.OrderId});
        }
    }

    class StartProcessingOrderHandler : IHandleMessages<StartProcessingOrder>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }

        public IBus Bus { get; set; }

        public void Handle(StartProcessingOrder message)
        {
            var order = NHibernateStorageContext.Session.Get<Entities.Order>(message.OrderId);

            order.Prepared = true;

            NHibernateStorageContext.Session.Save(order);

            Bus.Reply(new OrderIsReadyToBeShipped { OrderId = message.OrderId });
        }
    }

    class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        public NHibernateStorageContext NHibernateStorageContext { get; set; }
        public IBus Bus { get; set; }

        public void Handle(OrderPlaced message)
        {
            Console.Out.WriteLine("Order #{0} being shipped now", message.OrderId);

            var order = NHibernateStorageContext.Session.Get<Entities.Order>(message.OrderId);

            order.Shipped = true;

            NHibernateStorageContext.Session.Update(order);

            Bus.SendLocal(new OrderComplete { OrderId = message.OrderId });
        }
    }

    class OrderCompleteHandler : IHandleMessages<OrderComplete>
    {
        public static int NumExpectedMessages;
        public static int MessagesCounter;
        public static DateTime TimeStarted;
        static DateTime TimeEnded;

        public void Handle(OrderComplete message)
        {
            if (Interlocked.Increment(ref MessagesCounter) == NumExpectedMessages)
            {
                TimeEnded = DateTime.UtcNow;
                Console.WriteLine("Test finished, total time: {0:G}", TimeEnded - TimeStarted);
            }
        }
    }
}