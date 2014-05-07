using System.Collections.Generic;
using System.Configuration;
using NHibernate.Mapping.ByCode;
using Test.NHibernate.Entities;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Test.NHibernate
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization, UsingTransport<SqlServer>
    {
        public void Init()
        {
            var configuration = BuildConfiguration();

            Configure.Transactions.Advanced(t => t.DisableDistributedTransactions());

            Configure.With()
                .DefaultBuilder()
                .UseNHibernateTimeoutPersister()
                .UseNHibernateSagaPersister(configuration);
        }

        private static Configuration BuildConfiguration()
        {
            var configuration = new Configuration()
                .SetProperties(new Dictionary<string, string>
                {
                    {
                        Environment.ConnectionString,
                        ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString
                    },
                    {
                        Environment.Dialect,
                        "NHibernate.Dialect.MsSql2012Dialect"
                    }
                });

            var mapper = new ModelMapper();
            mapper.AddMapping<OrderMap>();
            var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddMapping(mappings);
            return configuration;
        }
    }
}
