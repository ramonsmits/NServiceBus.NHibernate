using NHibernate;
using NServiceBus;
using NServiceBus.Pipeline;

namespace Test.NHibernate
{
    abstract class HandlerWithNHibernateSession<T> : IHandleMessages<T>
    {
        public PipelineExecutor PipelineExecutor { get; set; }

        public IBus Bus { get; set; }

        public ISession Session
        {
            get { return PipelineExecutor.CurrentContext.Get<ISession>(); }
        }

        public abstract void Handle(T message);
    }
}