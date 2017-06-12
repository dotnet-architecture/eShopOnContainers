namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    public abstract class Rule
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public Campaign Campaign { get; set; }
        
        public string Description { get; set; }
    }


    public class UserProfileRule : Rule
    {
    }

    public class PurchaseHistoryRule : Rule
    {
    }

    public class UserLocationRule : Rule
    {
        public int LocationId { get; set; }
    }
}