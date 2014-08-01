using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, UsingTransport<Msmq>
    {
        public void Customize(ConfigurationBuilder builder)
        {
        }
    }
}