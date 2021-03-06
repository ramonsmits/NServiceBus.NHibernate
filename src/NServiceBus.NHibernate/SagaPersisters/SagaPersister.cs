namespace NServiceBus.SagaPersisters.NHibernate
{
    using System;
    using System.Linq;
    using global::NHibernate;
    using global::NHibernate.Criterion;
    using Persistence.NHibernate;
    using Saga;

    /// <summary>
    /// Saga persister implementation using NHibernate.
    /// </summary>
    class SagaPersister : ISagaPersister
    {
        
        public SagaPersister(IStorageSessionProvider storageSessionProvider)
        {
            this.storageSessionProvider = storageSessionProvider;
        }

      
        /// <summary>
        /// Saves the given saga entity using the current session of the
        /// injected session factory.
        /// </summary>
        /// <param name="saga">the saga entity that will be saved.</param>
        public void Save(IContainSagaData saga)
        {
            storageSessionProvider.ExecuteInTransaction(x => x.Save(saga));
        }

        
        /// <summary>
        /// Updates the given saga entity using the current session of the
        /// injected session factory.
        /// </summary>
        /// <param name="saga">the saga entity that will be updated.</param>
        public void Update(IContainSagaData saga)
        {
            storageSessionProvider.ExecuteInTransaction(x => x.Update(saga));
        }

        /// <summary>
        /// Gets a saga entity from the injected session factory's current session
        /// using the given saga id.
        /// </summary>
        /// <param name="sagaId">The saga id to use in the lookup.</param>
        /// <returns>The saga entity if found, otherwise null.</returns>
        public T Get<T>(Guid sagaId) where T : IContainSagaData
        {
            var result = default(T);
            storageSessionProvider.ExecuteInTransaction(x => result = x.Get<T>(sagaId, GetLockModeForSaga<T>()));
            return result;
        }

        T ISagaPersister.Get<T>(string property, object value)
        {
            var result = default(T);
            storageSessionProvider.ExecuteInTransaction(x => result = x.CreateCriteria(typeof(T))
                 .SetLockMode(GetLockModeForSaga<T>())
                 .Add(Restrictions.Eq(property, value))
                .UniqueResult<T>());
            return result;
        }

        /// <summary>
        /// Deletes the given saga from the injected session factory's
        /// current session.
        /// </summary>
        /// <param name="saga">The saga entity that will be deleted.</param>
        public void Complete(IContainSagaData saga)
        {
            storageSessionProvider.ExecuteInTransaction(x => x.Delete(saga));
        }

        LockMode GetLockModeForSaga<T>()
        {
            var explicitLockModeAttribute = typeof(T).GetCustomAttributes(typeof(LockModeAttribute), false).SingleOrDefault();

            if (explicitLockModeAttribute == null)
                return LockMode.Upgrade;//our new default in v4.1.0

            var explicitLockMode = ((LockModeAttribute)explicitLockModeAttribute).RequestedLockMode;

            switch (explicitLockMode)
            {
                case LockModes.Force:
                    return LockMode.Force;
                case LockModes.None:
                    return LockMode.None;
                case LockModes.Read:
                    return LockMode.Read;
                case LockModes.Upgrade:
                    return LockMode.Upgrade;
                case LockModes.UpgradeNoWait:
                    return LockMode.UpgradeNoWait;
                case LockModes.Write:
                    return LockMode.Write;

                default:
                    throw new InvalidOperationException("Unknown lock mode requested: " + explicitLockMode);

            }
        }

        readonly IStorageSessionProvider storageSessionProvider;
    }
}
