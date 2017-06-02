using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Exceptions;
using System;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Dto
{
    public enum RuleTypeEnum { UserProfileRule = 1, PurchaseHistoryRule = 2, UserLocationRule = 3 }

    public static class RuleType
    {
        public static RuleTypeEnum From(int id)
        {
            if (!Enum.IsDefined(typeof(RuleTypeEnum), id))
            {
                throw new MarketingDomainException($"Invalid value for RuleType, RuleTypeId: {id}");
            }

            return (RuleTypeEnum)id;
        }
    }
}