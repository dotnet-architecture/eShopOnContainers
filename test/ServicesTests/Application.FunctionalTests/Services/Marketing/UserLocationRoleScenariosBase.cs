﻿namespace FunctionalTests.Services.Marketing
{
    public class UserLocationRoleScenariosBase : MarketingScenariosBase
    {
        private const string EndpointLocationName = "locations";
        public static class Get
        {
            public static string UserLocationRulesByCampaignId(int campaignId)
                => GetUserLocationRolesUrlBase(campaignId);

            public static string UserLocationRuleByCampaignAndUserLocationRuleId(int campaignId,
                int userLocationRuleId)
                => $"{GetUserLocationRolesUrlBase(campaignId)}/{userLocationRuleId}";
        }

        public static class Post
        {
            public static string AddNewuserLocationRule(int campaignId) 
                => GetUserLocationRolesUrlBase(campaignId);
        }

        public static class Put
        {
            public static string UserLocationRoleBy(int campaignId,
                int userLocationRuleId)
                => $"{GetUserLocationRolesUrlBase(campaignId)}/{userLocationRuleId}";
        }

        public static class Delete
        {
            public static string UserLocationRoleBy(int campaignId,
                int userLocationRuleId)
                => $"{GetUserLocationRolesUrlBase(campaignId)}/{userLocationRuleId}";
        }


        private static string GetUserLocationRolesUrlBase(int campaignId)
            => $"{CampaignsUrlBase}/{campaignId}/{EndpointLocationName}";
    }
}