﻿namespace Runner.Saga
{
    using System;
    using NServiceBus.Saga;
    //using NServiceBus.SagaPersisters.NHibernate;

    //enable the below attribute to play with different lock modes
    //[LockMode(LockModes.None)]
    public class SagaData : IContainSagaData
    {
        public virtual string Originator { get; set; }

        public virtual string OriginalMessageId { get; set; }

        public virtual Guid Id { get; set; }

        [Unique]
        public virtual int Number { get; set; }

        public virtual int NumCalls { get; set; }
    }
}