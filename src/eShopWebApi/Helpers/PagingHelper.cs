using eShopWebApi.Core.Tools;
using eShopWebApi.Extensions;
using Microsoft.AspNetCore.Http;

namespace eShopWebApi.Helpers
{
    // TODO: Cover creating of paging information with unit tests.
    public class PagingHelper : IPagingHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPagingConfigurationCalculator _pagingCalculator;

        public PagingHelper(IHttpContextAccessor httpContextAccessor, IPagingConfigurationCalculator pagingCalculator)
        {
            _httpContextAccessor = Guard.ReturnIfChecked(httpContextAccessor, nameof(httpContextAccessor));
            _pagingCalculator = Guard.ReturnIfChecked(pagingCalculator, nameof(pagingCalculator));
        }

        public void UpdateHttpHeadersWithPagingInformations(int totalItemsCount)
        {
            AddHeader(HttpConstants.HeaderPagingTotalCount, totalItemsCount.ToString());
        }

        public void UpdateHttpHeadersWithPagingInformations(int totalItemsCount, int offset, int currentPageLimit)
        {
            // Total count
            UpdateHttpHeadersWithPagingInformations(totalItemsCount);

            // Current page link
            var page = new PagingConfiguration(offset, currentPageLimit);
            var pageUrl = CreateUrlWithPaging(page);
            AddHeader(HttpConstants.HeaderPagingCurrentPage, pageUrl);

            // Previous page link
            page = _pagingCalculator.GetConfigurationForPreviousPage(offset, currentPageLimit, totalItemsCount);
            pageUrl = CreateUrlWithPaging(page);
            AddHeader(HttpConstants.HeaderPagingPreviousPage, pageUrl);

            // Next page link
            page = _pagingCalculator.GetConfigurationForNextPage(offset, currentPageLimit, totalItemsCount);
            pageUrl = CreateUrlWithPaging(page);
            AddHeader(HttpConstants.HeaderPagingNextPage, pageUrl);
        }

        private string CreateUrlWithPaging(PagingConfiguration page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            var uriBuilder = _httpContextAccessor.HttpContext.Request.GetUriBuilderWithCurrentBaseUrl();
            uriBuilder.Query = $"{HttpConstants.UrlQueryParamOffset}={page.Offset}&{HttpConstants.UrlQueryParamLimit}={page.Limit}";

            return uriBuilder.Uri.ToString();
        }

        private void AddHeader(string key, string value)
        {
            _httpContextAccessor.HttpContext.Response.Headers[key] = value;
        }
    }
}
