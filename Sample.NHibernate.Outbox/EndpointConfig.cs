using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, UsingTransport<NServiceBus.RabbitMQ>
    {
        public void Customize(ConfigurationBuilder builder)
        {
        }
    }
}