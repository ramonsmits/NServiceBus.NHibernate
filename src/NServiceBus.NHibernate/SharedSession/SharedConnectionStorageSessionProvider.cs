namespace NServiceBus.Persistence.NHibernate
{
    using System;
    using System.Data;
    using global::NHibernate;
    using NServiceBus.ObjectBuilder;
    using Outbox;
    using Pipeline;

    class SharedConnectionStorageSessionProvider : IStorageSessionProvider
    {
        readonly IDbConnectionProvider connectionProvider;
        readonly SessionFactoryProvider sessionFactoryProvider;
        readonly IBuilder builder;
        
        public string ConnectionString { get; set; }

        public SharedConnectionStorageSessionProvider(IDbConnectionProvider connectionProvider, SessionFactoryProvider sessionFactoryProvider, IBuilder builder)
        {
            this.connectionProvider = connectionProvider;
            this.sessionFactoryProvider = sessionFactoryProvider;
            this.builder = builder;
        }

        public ISession Session
        {
            get
            {
                Lazy<ISession> existingSession;

                if (!builder.Build<BehaviorContext>().TryGet(string.Format("LazyNHibernateSession-{0}", ConnectionString), out existingSession))
                {
                    throw new Exception("No active storage session found in context");
                }

                return existingSession.Value;
            }
        }

        public IStatelessSession OpenStatelessSession()
        {
            IDbConnection connection;

            if (connectionProvider.TryGetConnection(out connection, ConnectionString))
            {
                return sessionFactoryProvider.SessionFactory.OpenStatelessSession(connection);
            }

            return sessionFactoryProvider.SessionFactory.OpenStatelessSession();
        }

        public ISession OpenSession()
        {
            IDbConnection connection;

            if (connectionProvider.TryGetConnection(out connection, ConnectionString))
            {
                return sessionFactoryProvider.SessionFactory.OpenSession(connection);
            }

            return sessionFactoryProvider.SessionFactory.OpenSession();
        }

        public void ExecuteInTransaction(Action<ISession> operation)
        {
            operation(Session);
        }
    }
}