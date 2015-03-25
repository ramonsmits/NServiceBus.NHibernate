namespace NServiceBus.Persistence.NHibernate
{
    using System;
    using System.Data;
    using global::NHibernate;
    using Pipeline;

    /// <summary>
    /// Provides users with access to the current NHibernate <see cref="ITransaction"/>, <see cref="IDbConnection"/> and <see cref="ISession"/>. 
    /// </summary>
    public class NHibernateStorageContext
    {
        readonly BehaviorContext behaviorContext;
        readonly string connectionString;

        internal NHibernateStorageContext(BehaviorContext behaviorContext, string connectionString)
        {
            this.behaviorContext = behaviorContext;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the current context NHibernate <see cref="IDbConnection"/>.
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                Lazy<IDbConnection> lazy;
                if (behaviorContext.TryGet(string.Format("LazySqlConnection-{0}", connectionString), out lazy))
                {
                    return lazy.Value;
                }

                throw new InvalidOperationException("No connection available");
            }
        }

        /// <summary>
        /// Gets the current context NHibernate <see cref="ISession"/>.
        /// </summary>
        public ISession Session
        {
            get
            {
                Lazy<ISession> lazy;
                if (behaviorContext.TryGet(string.Format("LazyNHibernateSession-{0}", connectionString), out lazy))
                {
                    return lazy.Value;
                }

                throw new InvalidOperationException("No session available");
            }
        }

        /// <summary>
        /// Gets the current context NHibernate <see cref="ITransaction"/>.
        /// </summary>
        public ITransaction Transaction
        {
            get
            {
                Lazy<ITransaction> lazy;
                if (behaviorContext.TryGet(string.Format("LazyNHibernateTransaction-{0}", connectionString), out lazy))
                {
                    return lazy.Value;
                }

                throw new InvalidOperationException("No transaction available");
            }
        }
    }
}