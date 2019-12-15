using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Webhooks.API.Model;

namespace Webhooks.API.Controllers
{
    public class WebhookSubscriptionRequest : IValidatableObject
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string Event { get; set; }
        public string GrantUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Uri.IsWellFormedUriString(GrantUrl, UriKind.Absolute))
            {
                yield return new ValidationResult("GrantUrl is not valid", new[] { nameof(GrantUrl) });
            }

            if (!Uri.IsWellFormedUriString(Url, UriKind.Absolute))
            {
                yield return new ValidationResult("Url is not valid", new[] { nameof(Url) });
            }

            var isOk = Enum.TryParse<WebhookType>(Event, ignoreCase: true, result: out WebhookType whtype);
            if (!isOk)
            {
                yield return new ValidationResult($"{Event} is invalid event name", new[] { nameof(Event) });
            }
        }

    }
}
