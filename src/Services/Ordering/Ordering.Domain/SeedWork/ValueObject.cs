
namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;


    public class ValueObject<TValueObject> : IEquatable<TValueObject>
        where TValueObject : ValueObject<TValueObject>
    {
        //A ValueObject doesn't have Identity, but we just need an Id/key so EF knows how to persist
        //becuase in EF Core it still doesn't support ValueObjects or ComplexTypes
        //This should be changed when EF Core supports any of those. 
        // https://github.com/aspnet/EntityFramework/issues/246
        public virtual Guid Id { get; protected set; }

        //IEquatable and Override Equals operators

        
        public bool Equals(TValueObject other)
        {
            if ((object)other == null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            //compare all public properties
            PropertyInfo[] publicProperties = this.GetType().GetProperties();

            if ((object)publicProperties != null
                &&
                publicProperties.Any())
            {
                return publicProperties.All(p =>
                {
                    var left = p.GetValue(this, null);
                    var right = p.GetValue(other, null);


                    if (typeof(TValueObject).IsAssignableFrom(left.GetType()))
                    {
                        //check not self-references...
                        return Object.ReferenceEquals(left, right);
                    }
                    else
                        return left.Equals(right);


                });
            }
            else
                return true;
        }
       
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            ValueObject<TValueObject> item = obj as ValueObject<TValueObject>;

            if ((object)item != null)
                return Equals((TValueObject)item);
            else
                return false;

        }
       
        public override int GetHashCode()
        {
            int hashCode = 31;
            bool changeMultiplier = false;
            int index = 1;

            //compare all public properties
            PropertyInfo[] publicProperties = this.GetType().GetProperties();


            if ((object)publicProperties != null
                &&
                publicProperties.Any())
            {
                foreach (var item in publicProperties)
                {
                    object value = item.GetValue(this, null);

                    if ((object)value != null)
                    {

                        hashCode = hashCode * ((changeMultiplier) ? 59 : 114) + value.GetHashCode();

                        changeMultiplier = !changeMultiplier;
                    }
                    else
                        hashCode = hashCode ^ (index * 13);//only for support {"a",null,null,"a"} <> {null,"a","a",null}
                }
            }

            return hashCode;
        }
        
        public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);

        }
       
        public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        {
            return !(left == right);
        }


    }
}

