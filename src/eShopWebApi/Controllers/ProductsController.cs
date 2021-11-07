using AutoMapper;
using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.Helpers;
using eShopWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eShopWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IPagingHelper _pagingHelper;

        public ProductsController(IProductService productService, IPagingHelper pagingHelper, IMapper mapper)
        {
            _productService = Guard.ReturnIfChecked(productService, nameof(productService));
            _mapper = Guard.ReturnIfChecked(mapper, nameof(mapper));
            _pagingHelper = Guard.ReturnIfChecked(pagingHelper, nameof(pagingHelper));
        }


        [HttpGet]
        [MapToApiVersion("1.0")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingTotalCount, "integer", "Total number of all items")]
        public async Task<ICollection<GetProductResponse>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            var totalProductsCount = await _productService.GetProductsTotalCountAsync();

            _pagingHelper.UpdateHttpHeadersWithPagingInformations(totalProductsCount);
            
            return _mapper.Map<List<GetProductResponse>>(products);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingTotalCount, "integer", "Total number of all items")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingCurrentPage, "string", "URL to current page")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingPreviousPage, "string", "URL to previous page")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingNextPage, "string", "URL to next page")]
        public async Task<ICollection<GetProductResponse>> GetProductsPaginated(int offset = 0, int limit = 3)
        {
            var products = await _productService.GetProductsAsync(offset, limit);
            var totalProductsCount = await _productService.GetProductsTotalCountAsync();

            _pagingHelper.UpdateHttpHeadersWithPagingInformations(totalProductsCount, offset, limit);

            return _mapper.Map<List<GetProductResponse>>(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetProductResponse>>GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
