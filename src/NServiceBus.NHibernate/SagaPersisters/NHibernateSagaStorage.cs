namespace NServiceBus.Features
{
    using System;
    using System.Linq;
    using NHibernate.Cfg;
    using NServiceBus.Saga;
    using SagaPersisters.NHibernate;
    using SagaPersisters.NHibernate.AutoPersistence;
    using Settings;

    /// <summary>
    /// NHibernate Saga Storage.
    /// </summary>
    public class NHibernateSagaStorage : Feature
    {
        /// <summary>
        /// Creates an instance of <see cref="NHibernateSagaStorage"/>.
        /// </summary>
        public NHibernateSagaStorage()
        {
            DependsOn<Sagas>();
        }

        /// <summary>
        /// Called when the feature should perform its initialization. This call will only happen if the feature is enabled.
        /// </summary>
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Settings.Get<SharedMappings>()
                .AddMapping(configuration => ApplyMappings(context.Settings, configuration));

            context.Container.ConfigureComponent<SagaPersister>(DependencyLifecycle.SingleInstance);
        }

        internal void ApplyMappings(ReadOnlySettings settings, Configuration configuration)
        {
            var tableNamingConvention = settings.GetOrDefault<Func<Type, string>>("NHibernate.Sagas.TableNamingConvention");

            var scannedAssemblies = settings.GetAvailableTypes().Select(t => t.Assembly).Distinct();

            foreach (var assembly in scannedAssemblies)
            {
                configuration.AddAssembly(assembly);
            }

            var sagaMetaModel = settings.Get<SagaMetaModel>();
            var types = settings.GetAvailableTypes().Except(configuration.ClassMappings.Select(x => x.MappedClass));
            SagaModelMapper modelMapper;
            if (tableNamingConvention == null)
            {
                modelMapper = new SagaModelMapper(sagaMetaModel, types);
            }
            else
            {
                modelMapper = new SagaModelMapper(sagaMetaModel, types, tableNamingConvention);
            }

            configuration.AddMapping(modelMapper.Compile());
        }
    }
}