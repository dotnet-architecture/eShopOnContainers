namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    public abstract class Rule
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public Campaign Campaign { get; set; }
        
        public string Description { get; set; }

        public abstract int RuleTypeId { get;}
    }


    public class UserProfileRule : Rule
    {
        public override int RuleTypeId => (int)RuleTypeEnum.UserProfileRule;
    }

    public class PurchaseHistoryRule : Rule
    {
        public override int RuleTypeId => (int)RuleTypeEnum.PurchaseHistoryRule;
    }

    public class UserLocationRule : Rule
    {
        public override int RuleTypeId => (int)RuleTypeEnum.UserLocationRule;

        public int LocationId { get; set; }
    }
}