using System.Collections.Generic;
using System.Configuration;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NServiceBus;
using NServiceBus.Persistence;
using Sample.NHibernate.Outbox.Entities;
using Configuration = NHibernate.Cfg.Configuration;

namespace Sample.NHibernate.Outbox
{
    public class EnableNoDTC : INeedInitialization
    {
        public void Customize(BusConfiguration config)
        {
            var configuration = BuildConfiguration();

            config
                .UsePersistence<NHibernatePersistence>()
                .UseConfiguration(configuration);
            config.EnableOutbox();
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