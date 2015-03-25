namespace NServiceBus.Persistence.NHibernate
{
    using System;
    using System.Transactions;
    using global::NHibernate;
    using Pipeline;

    class OpenNativeTransactionBehavior : PhysicalMessageProcessingStageBehavior
    {
        readonly SharedConnectionStorageSessionProvider sharedConnectionStorageSessionProvider;

        public string ConnectionString { get; set; }

        public OpenNativeTransactionBehavior(SharedConnectionStorageSessionProvider sharedConnectionStorageSessionProvider)
        {
            this.sharedConnectionStorageSessionProvider = sharedConnectionStorageSessionProvider;
        }

        public override void Invoke(Context context, Action next)
        {
            if (Transaction.Current != null)
            {
                next();
                return;
            }

            var lazyTransaction = new Lazy<ITransaction>(() => sharedConnectionStorageSessionProvider.Session.BeginTransaction());
            context.Set(string.Format("LazyNHibernateTransaction-{0}", ConnectionString), lazyTransaction);

            try
            {
                next();

                if (lazyTransaction.IsValueCreated)
                {
                    if (lazyTransaction.Value.IsActive)
                    {
                        lazyTransaction.Value.Commit();
                    }
                }
            }
            finally
            {
                context.Remove(string.Format("LazyNHibernateTransaction-{0}", ConnectionString));
            }
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("OpenNHibernateTransaction", typeof(OpenNativeTransactionBehavior), "Makes sure that there is a NHibernate ITransaction wrapping the pipeline")
            {
                InsertAfter(WellKnownStep.ExecuteUnitOfWork);
                InsertBeforeIfExists("OutboxRecorder");
            }
        }
    }
}
