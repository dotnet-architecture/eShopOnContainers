using Newtonsoft.Json;

namespace eShopOnContainers.Core.Models.User
{
    public class UserInfo
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("card_number")]
        public string CardNumber { get; set; }

        [JsonProperty("card_holder")]
        public string CardHolder { get; set; }

        [JsonProperty("card_security_number")]
        public string CardSecurityNumber { get; set; }

        [JsonProperty("address_city")]
        public string Address { get; set; }

        [JsonProperty("address_country")]
        public string Country { get; set; }

        [JsonProperty("address_state")]
        public string State { get; set; }

        [JsonProperty("address_street")]
        public string Street { get; set; }

        [JsonProperty("address_zip_code")]
        public string ZipCode { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }
    }
}
