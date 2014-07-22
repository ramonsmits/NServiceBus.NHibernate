using System;
using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    class NewOrder : ICommand
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
    }

    class OrderComplete : ICommand
    {
        public Guid OrderId { get; set; }
    }

    class OrderIsReadyToBeShipped : IMessage
    {
        public Guid OrderId { get; set; }
    }

    class StartBuyersRemorse : ICommand
    {
        public Guid OrderId { get; set; }
    }

    class BuyersRemorseIsOver: IMessage
    {
        public Guid OrderId { get; set; }
    }

    class StartProcessingOrder : ICommand
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }        
    }
    
    class OrderPlaced : IMessage
    {
        public Guid OrderId { get; set; }
    }
}