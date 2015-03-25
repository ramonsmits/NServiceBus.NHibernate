namespace NServiceBus.Deduplication.NHibernate
{
    using System;
    using System.Data;
    using Config;
    using Gateway.Deduplication;
    using global::NHibernate;
    using global::NHibernate.Exceptions;
    using Persistence.NHibernate;

    class GatewayDeduplication : IDeduplicateMessages
    {
        readonly ISessionFactory sessionFactory;

        public GatewayDeduplication(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public bool DeduplicateMessage(string clientId, DateTime timeReceived)
        {
            using (var conn = sessionFactory.GetConnection())
            using (var session = sessionFactory.OpenSessionEx(conn))
            using (var tx = session.BeginAmbientTransactionAware(IsolationLevel.ReadCommitted))
            {
                var gatewayMessage = session.Get<DeduplicationMessage>(clientId);

                if (gatewayMessage != null)
                {
                    tx.Commit();
                    return false;
                }

                gatewayMessage = new DeduplicationMessage
                {
                    Id = clientId,
                    TimeReceived = timeReceived
                };

                try
                {
                    session.Save(gatewayMessage);
                    tx.Commit();
                }
                catch (GenericADOException)
                {
                    tx.Rollback();
                    return false;
                }
            }

            return true;
        }
    }
}
