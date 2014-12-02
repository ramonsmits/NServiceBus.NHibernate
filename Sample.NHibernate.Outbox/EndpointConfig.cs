using NServiceBus;

namespace Sample.NHibernate.Outbox
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.UseTransport<RabbitMQTransport>();
        }
    }
}