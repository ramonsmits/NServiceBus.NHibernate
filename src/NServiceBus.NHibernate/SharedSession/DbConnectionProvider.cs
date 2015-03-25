namespace NServiceBus.Persistence.NHibernate
{
    using System;
    using System.Data;
    using Outbox;
    using Pipeline;

    class DbConnectionProvider : IDbConnectionProvider
    {
        readonly BehaviorContext context;

        public string DefaultConnectionString { get; set; }
        public bool DisableConnectionSharing { get; set; }

        public DbConnectionProvider(BehaviorContext context)
        {
            this.context = context;
        }

        public bool TryGetConnection(out IDbConnection connection, string connectionString)
        {
            if (DisableConnectionSharing)
            {
                connection = null;
                return false;
            }
            if (connectionString == null)
            {
                connectionString = DefaultConnectionString;
            }

            var result = context.TryGet(string.Format("SqlConnection-{0}", connectionString), out connection);

            if (result == false)
            {
                Lazy<IDbConnection> lazyConnection;

                result = context.TryGet(string.Format("LazySqlConnection-{0}", connectionString), out lazyConnection);
                
                if (result)
                {
                    connection = lazyConnection.Value;
                }
            }

            return result;
        }
    }
}