namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class RuleType
    {
        public static readonly RuleType UserProfileRule = new RuleType(1, nameof(UserProfileRule));
        public static readonly RuleType PurchaseHistoryRule = new RuleType(2, nameof(UserProfileRule));
        public static readonly RuleType UserLocationRule = new RuleType(3, nameof(UserProfileRule));

        public readonly int Id;
        public readonly string Name;

        private RuleType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static IEnumerable<RuleType> List() =>
    new[] { UserProfileRule, PurchaseHistoryRule, UserLocationRule };

        public static RuleType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new MarketingDomainException($"Possible values for RuleType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static RuleType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new MarketingDomainException($"Possible values for RuleType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}