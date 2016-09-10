using System;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
        public class Buyer : AggregateRoot
        {
            public Buyer(Guid buyerId, string name, string lastName, string email, Address address, string phoneNumber)
            {
                this.Id = buyerId;
                this.Name = name;
                this.LastName = lastName;
                this.Email = email;
                this.Address = address;
                this.PhoneNumber = phoneNumber;
            }


            public virtual string Name
            {
                get;
                private set;
            }

            public virtual string LastName
            {
                get;
                private set;
            }

            public virtual string Email
            {
                get;
                private set;
            }

            public virtual Address Address
            {
                get;
                private set;
            }

            public virtual string PhoneNumber
            {
                get;
                private set;
            }
    }
}
