using eShopWebApi.Core.Tools;
using eShopWebApi.Extensions;
using Microsoft.AspNetCore.Http;

namespace eShopWebApi.Helpers
{
    public class PagingHelper : IPagingHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagingHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = Guard.ReturnIfChecked(httpContextAccessor, nameof(httpContextAccessor));
        }

        public void UpdateHttpHeadersWithPagingInformations(int totalItemsCount)
        {
            AddHeader(HttpConstants.HeaderPagingTotalCount, totalItemsCount.ToString());
        }

        public void UpdateHttpHeadersWithPagingInformations(int totalItemsCount, int offset, int currentPageLimit)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string tmpParamValue;

            // Total count
            UpdateHttpHeadersWithPagingInformations(totalItemsCount);

            // Current page link
            _httpContextAccessor.HttpContext.Response.Headers[HttpConstants.HeaderPagingCurrentPage] = request.GetFullPagingUrl().ToString();
            AddHeader(HttpConstants.HeaderPagingCurrentPage, request.GetFullPagingUrl().ToString());
            
            // Previous page link
            var uriBuilder = request.GetUriBuilderWithCurrentBaseUrl();
            if (offset > 0)
            {
                var prevOffset = offset - currentPageLimit;
                var prevLimit = 0;
                if (prevOffset < 0)
                {
                    prevLimit = offset;
                    prevOffset = 0;
                }
                else
                {
                    prevLimit = currentPageLimit;
                }
                uriBuilder.Query = $"{HttpConstants.UrlQueryParamOffset}={prevOffset}&{HttpConstants.UrlQueryParamLimit}={prevLimit}";
                tmpParamValue = uriBuilder.Uri.ToString();
            }
            else
            {
                tmpParamValue = string.Empty;
            }
            AddHeader(HttpConstants.HeaderPagingPreviousPage, tmpParamValue);


            // Next page link
            uriBuilder = request.GetUriBuilderWithCurrentBaseUrl();
            var nextOffset = offset + currentPageLimit;
            if(nextOffset < totalItemsCount)
            {
                uriBuilder.Query = $"{HttpConstants.UrlQueryParamOffset}={nextOffset}&{HttpConstants.UrlQueryParamLimit}={currentPageLimit}";
                tmpParamValue = uriBuilder.Uri.ToString();
            }
            else
            {
                tmpParamValue = string.Empty;
            }
            AddHeader(HttpConstants.HeaderPagingNextPage, tmpParamValue);
        }

        private void AddHeader(string key, string value)
        {
            _httpContextAccessor.HttpContext.Response.Headers[key] = value;
        }
    }
}
