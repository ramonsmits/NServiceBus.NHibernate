namespace NServiceBus.Persistence.NHibernate
{
    using System;
    using System.Data;
    using global::NHibernate;
    using Outbox;
    using Pipeline;

    class SharedConnectionStorageSessionProvider : IStorageSessionProvider
    {
        readonly IDbConnectionProvider connectionProvider;
        readonly SessionFactoryProvider sessionFactoryProvider;
        readonly BehaviorContext context;
        
        public string ConnectionString { get; set; }

        public SharedConnectionStorageSessionProvider(IDbConnectionProvider connectionProvider, SessionFactoryProvider sessionFactoryProvider, BehaviorContext context)
        {
            this.connectionProvider = connectionProvider;
            this.sessionFactoryProvider = sessionFactoryProvider;
            this.context = context;
        }

        public ISession Session
        {
            get
            {
                Lazy<ISession> existingSession;

                if (!context.TryGet(string.Format("LazyNHibernateSession-{0}", ConnectionString), out existingSession))
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