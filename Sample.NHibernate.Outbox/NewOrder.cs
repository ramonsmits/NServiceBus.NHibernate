using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    internal class NewOrder : ICommand
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }
}