using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;
using System;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Extensions
{
    public static class SpecificationExtentions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> spec1, ISpecification<T> spec2) => new AndSpecification<T>(spec1, spec2);
    }

    public class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _spec1;
        private readonly ISpecification<T> _spec2;

        public AndSpecification(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            if (spec1 == null)
                throw new ArgumentNullException("spec1");

            if (spec2 == null)
                throw new ArgumentNullException("spec2");

            _spec1 = spec1;
            _spec2 = spec2;
        }

        public bool IsSatisfiedBy(T candidate) => _spec1.IsSatisfiedBy(candidate) && _spec2.IsSatisfiedBy(candidate);
    }
}
