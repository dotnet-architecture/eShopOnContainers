namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork
{
    using System;
    using MediatR;
    using System.Collections.Generic;

    public abstract class Entity
    {

        int? _requestedHashCode;
        int _Id;

        private List<IAsyncNotification> _events;

        public virtual  int Id 
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

        public List<IAsyncNotification> Events => _events;        
        public void AddEvent(IAsyncNotification eventItem)
        {
            _events = _events ?? new List<IAsyncNotification>();
            _events.Add(eventItem);
        }

        public void RemoveEvent(IAsyncNotification eventItem)
        {
            if (_events is null) return;
            _events.Remove(eventItem);
        }

        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

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

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
