namespace NServiceBus.Unicast.Subscriptions.NHibernate
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    class CachedSubscriptionPersister : SubscriptionPersister
    {
        public CachedSubscriptionPersister(SubscriptionStorageSessionProvider subscriptionStorageSessionProvider, TimeSpan expiration) : base(subscriptionStorageSessionProvider)
        {
            this.expiration = expiration;
        }

        public override void Subscribe(string address, IEnumerable<MessageType> messageTypes)
        {
            base.Subscribe(address, messageTypes);
            cache.Clear();
        }

        public override void Unsubscribe(string address, IEnumerable<MessageType> messageTypes)
        {
            base.Unsubscribe(address, messageTypes);
            cache.Clear();
        }

        public override IEnumerable<string> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
        {
            var types = messageTypes.ToList();
            var typeNames = types.Select(mt => mt.TypeName).ToArray();
            var key = String.Join(",", typeNames);
            Tuple<DateTimeOffset, IEnumerable<string>> cacheItem;
            var cacheItemFound = cache.TryGetValue(key, out cacheItem);

            if (cacheItemFound && (DateTimeOffset.UtcNow - cacheItem.Item1) < expiration)
            {
                return cacheItem.Item2;
            }

            cacheItem = new Tuple<DateTimeOffset, IEnumerable<string>>(
                DateTimeOffset.UtcNow,
                base.GetSubscriberAddressesForMessage(types)
                );

            cache.AddOrUpdate(key, s => cacheItem, (s, tuple) => cacheItem);

            return cacheItem.Item2;
        }

        static readonly ConcurrentDictionary<string, Tuple<DateTimeOffset, IEnumerable<string>>> cache = new ConcurrentDictionary<string, Tuple<DateTimeOffset, IEnumerable<string>>>();
        TimeSpan expiration;
    }
}