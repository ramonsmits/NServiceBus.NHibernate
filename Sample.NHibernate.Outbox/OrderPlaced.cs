using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    internal class OrderPlaced : IMessage
    {
        public long OrderId { get; set; }
    }
}