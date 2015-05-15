namespace NServiceBus.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using NHibernate;
    using Persistence.NHibernate;

    class OutboxStorage : IStoreOutboxMessages
    {
        readonly IStorageSessionProvider storageSessionProvider;

        public OutboxStorage(IStorageSessionProvider storageSessionProvider)
        {
            this.storageSessionProvider = storageSessionProvider;
        }

        public void Store(string messageId, IEnumerable<TransportOperation> transportOperations)
        {
            var operations = transportOperations.Select(t => new OutboxOperation
            {
                Message = t.Body,
                Headers = t.Headers,
                MessageId = t.MessageId,
                Options = t.Options,
            });

            storageSessionProvider.ExecuteInTransaction(x => x.Save(new OutboxRecord
            {
                MessageId = messageId,
                Dispatched = false,
                TransportOperations = OutboxOperationSerialization.ConvertObjectToString(operations)
            }));
        }
    }
}
