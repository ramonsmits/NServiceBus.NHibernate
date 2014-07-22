using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Sample.NHibernate.Outbox.Entities
{
    internal class Order
    {
        public virtual Guid Id { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Product { get; set; }
        public virtual bool Shipped { get; set; }
        public virtual bool Prepared { get; set; }
    }

    internal class OrderMap : ClassMapping<Order>
    {
        public OrderMap()
        {
            Table("[Order]");
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(p => p.Quantity);
            Property(p => p.Product);
            Property(p => p.Prepared);
            Property(p => p.Shipped);
        }
    }
}