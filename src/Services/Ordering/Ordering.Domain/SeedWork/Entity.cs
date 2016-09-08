using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class Entity
    {
        //Members

        int? _requestedHashCode;
        Guid _Id;


        //Properties

        /// <summary>
        /// Get the persisten object identifier
        /// </summary>
        public virtual  Guid Id 
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        //Public Methods

        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns>True if entity is transient, else false</returns>
        public bool IsTransient()
        {
            return this.Id == Guid.Empty;
        }

        /// <summary>
        /// Generate identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if ( IsTransient())
                this.Id = IdentityGenerator.NewSequentialGuid();
        }

        /// <summary>
        /// Change current identity for a new non transient identity
        /// </summary>
        /// <param name="identity">the new identity</param>
        public void ChangeCurrentIdentity(Guid identity)
        {
            if ( identity != Guid.Empty)
                this.Id = identity;
        }


        //Overrides Methods

        /// <summary>
        /// <see cref="M:System.Object.Equals"/>
        /// </summary>
        /// <param name="obj"><see cref="M:System.Object.Equals"/></param>
        /// <returns><see cref="M:System.Object.Equals"/></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        /// <summary>
        /// <see cref="M:System.Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="M:System.Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        /// <summary>
        /// equals operator
        /// </summary>
        /// <param name="left">left parameter</param>
        /// <param name="right">right parameter</param>
        /// <returns>true if equals</returns>
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        /// <summary>
        /// Distinct operator
        /// </summary>
        /// <param name="left">left parameter</param>
        /// <param name="right">right parameter</param>
        /// <returns>True if different</returns>
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

    }
}
