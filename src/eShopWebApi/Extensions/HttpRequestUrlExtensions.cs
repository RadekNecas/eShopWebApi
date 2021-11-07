using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopWebApi.Extensions
{
    public static class HttpRequestUrlExtensions
    {
        /// <summary>
        /// <returns>Uri builder with constructed URL from current request without query and fragment parts.</returns>
        public static UriBuilder GetUriBuilderWithCurrentBaseUrl(this HttpRequest httpRequest)
        {
            var defaultPort = httpRequest.IsHttps ? 443 : 80;
            return new UriBuilder(httpRequest.Scheme, httpRequest.Host.Host, httpRequest.Host.Port ?? defaultPort, httpRequest.Path);
        }

        public static Uri GetFullPagingUrl(this HttpRequest httpRequest)
        {
            var builder = httpRequest.GetUriBuilderWithCurrentBaseUrl();
            var currentHeaders = httpRequest.Query;

            Microsoft.Extensions.Primitives.StringValues paramValues;
            var queryParamsCount = 0;
            var querySb = new StringBuilder();

            if (currentHeaders.TryGetValue(HttpConstants.UrlQueryParamOffset, out paramValues))
            {
                querySb.Append($"{HttpConstants.UrlQueryParamOffset}={paramValues}");
                queryParamsCount++;
            }
            if (currentHeaders.TryGetValue(HttpConstants.UrlQueryParamLimit, out paramValues))
            {
                if(queryParamsCount > 0)
                {
                    querySb.Append("&");
                }
                querySb.Append($"{HttpConstants.UrlQueryParamLimit}={paramValues}");
                queryParamsCount++;
            }

            if(queryParamsCount > 0)
            {
                builder.Query = querySb.ToString();
            }

            return builder.Uri;
        }

    }
}
