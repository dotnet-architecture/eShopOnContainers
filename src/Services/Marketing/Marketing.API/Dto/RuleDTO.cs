namespace Microsoft.eShopOnContainers.Services.Marketing.API.Dto
{
    public class RuleDTO
    {
        public int Id { get; set; }

        public RuleTypeEnum RuleType => (RuleTypeEnum) RuleTypeId;

        public int RuleTypeId { get; set; }

        public int CampaignId { get; set; }

        public int? LocationId { get; set; }

        public string Description { get; set; }
    }

    public enum RuleTypeEnum { UserProfileRule = 1, PurchaseHistoryRule = 2, UserLocationRule = 3 }
}