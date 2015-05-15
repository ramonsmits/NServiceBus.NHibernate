namespace NServiceBus.Outbox
{
    using System;
    using System.Data;
    using System.Linq;
    using global::NHibernate.Criterion;
    using NHibernate;
    using Persistence.NHibernate;

    class OutboxDeduplication : IDeduplicateMessages
    {
        readonly IStorageSessionProvider storageSessionProvider;

        public OutboxDeduplication(IStorageSessionProvider storageSessionProvider)
        {
            this.storageSessionProvider = storageSessionProvider;
        }

        public bool TryGet(string messageId, out OutboxMessage message)
        {
            OutboxRecord result;

            message = null;

            using (var session = storageSessionProvider.OpenStatelessSession())
            {
                using (var tx = session.BeginAmbientTransactionAware(IsolationLevel.ReadCommitted))
                {
                    //Explicitly using ICriteria instead of QueryOver for performance reasons.
                    //It seems QueryOver uses quite a bit reflection and that takes longer.
                    result = session.CreateCriteria<OutboxRecord>().Add(Expression.Eq("MessageId", messageId))
                        .UniqueResult<OutboxRecord>();

                    tx.Commit();
                }
            }

            if (result == null)
            {
                return false;
            }

            message = new OutboxMessage(result.MessageId);

            var operations = OutboxOperationSerialization.ConvertStringToObject(result.TransportOperations);
            message.TransportOperations.AddRange(operations.Select(t => new TransportOperation(t.MessageId, 
                t.Options, t.Message, t.Headers)));

            return true;
        }

        public void SetAsDispatched(string messageId)
        {
            using (var session = storageSessionProvider.OpenStatelessSession())
            {
                using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var queryString = string.Format("update {0} set Dispatched = true, DispatchedAt = :date where MessageId = :messageid And Dispatched = false",
                        typeof(OutboxRecord));
                    session.CreateQuery(queryString)
                        .SetString("messageid", messageId)
                        .SetDateTime("date", DateTime.UtcNow)
                        .ExecuteUpdate();

                    tx.Commit();
                }
            }
        }

        public void RemoveEntriesOlderThan(DateTime dateTime)
        {
            using (var session = storageSessionProvider.OpenSession())
            {
                using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var result = session.QueryOver<OutboxRecord>().Where(o => o.Dispatched && o.DispatchedAt < dateTime)
                        .List();

                    foreach (var record in result)
                    {
                        session.Delete(record);
                    }

                    tx.Commit();
                }
            }
        }
    }
}
