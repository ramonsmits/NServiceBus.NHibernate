using System;
using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    public class OrderIsReadyToBeShipped : IMessage
    {
        public Guid OrderId { get; set; }
    }

    public class OrderSubmitted : IEvent
    {
        public Guid OrderId { get; set; }        
    }
}