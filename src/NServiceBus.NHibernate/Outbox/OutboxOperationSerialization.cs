namespace NServiceBus.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.Outbox.NHibernate;
    using NServiceBus.Serializers.Json;

    static class OutboxOperationSerialization
    {
        internal static IEnumerable<OutboxOperation> ConvertStringToObject(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return Enumerable.Empty<OutboxOperation>();
            }

            return (IEnumerable<OutboxOperation>)serializer.DeserializeObject(data, typeof(IEnumerable<OutboxOperation>));
        }

        internal static string ConvertObjectToString(IEnumerable<OutboxOperation> operations)
        {
            if (operations == null || !operations.Any())
            {
                return null;
            }

            return serializer.SerializeObject(operations);
        }

        static readonly JsonMessageSerializer serializer = new JsonMessageSerializer(null);
    }
}
