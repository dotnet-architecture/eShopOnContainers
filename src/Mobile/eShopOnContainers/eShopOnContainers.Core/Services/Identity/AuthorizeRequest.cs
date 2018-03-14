using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace eShopOnContainers.Core.Services.Identity
{
    public class AuthorizeRequest
    {
        readonly Uri _authorizeEndpoint;

        public AuthorizeRequest(string authorizeEndpoint)
        {
            _authorizeEndpoint = new Uri(authorizeEndpoint);
        }

        public string Create(IDictionary<string, string> values)
        {
            var queryString = string.Join("&", values.Select(kvp => string.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value))).ToArray());
            return string.Format("{0}?{1}", _authorizeEndpoint.AbsoluteUri, queryString);
        }
    }
}
