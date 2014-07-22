using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    class NewOrder : ICommand
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }

    class ShowOrders : ICommand
    {
        
    }

    class OrderPlaced : IMessage
    {
        public long OrderId { get; set; }
    }
}