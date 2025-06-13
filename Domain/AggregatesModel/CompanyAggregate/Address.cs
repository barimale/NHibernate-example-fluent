﻿using Demo.Domain.Abstraction;

namespace Demo.Domain.AggregatesModel.CompanyAggregate
{
    public class Address: Entity<int>, IAggregateRoot
    {
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string ZipCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual IList<Company> Companies { get; set; } = new List<Company>();

    }
}
