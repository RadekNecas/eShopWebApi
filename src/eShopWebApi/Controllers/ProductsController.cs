﻿using AutoMapper;
using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.Helpers;
using eShopWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace eShopWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(InternalServerErrorResponse))]
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


        /// <summary>
        /// Get all products without paging support.
        /// Total number of products is returned in response header.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingTotalCount, "integer", "Total number of all items")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetProductResponse[]))]
        public async Task<ICollection<GetProductResponse>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            var totalProductsCount = await _productService.GetProductsTotalCountAsync();

            _pagingHelper.UpdateHttpHeadersWithPagingInformations(totalProductsCount);
            
            return _mapper.Map<List<GetProductResponse>>(products);
        }

        /// <summary>
        /// Returns products with paging support.
        /// Paging information are stored in response headers.
        /// </summary>
        /// <param name="offset">Inclusive offset from which to start the page. Starting from 0. Item on the offset position is included to the result.</param>
        /// <param name="limit">Limit for number of items on the page.</param>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("2.0")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingTotalCount, "integer", "Total number of all items")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingCurrentPage, "string", "URL to current page")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingPreviousPage, "string", "URL to previous page")]
        [SwaggerResponseHeader((int)HttpStatusCode.OK, HttpConstants.HeaderPagingNextPage, "string", "URL to next page")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetProductResponse[]))]
        public async Task<ICollection<GetProductResponse>> GetProductsPaginated(int offset = 0, int limit = 3)
        {
            var products = await _productService.GetProductsAsync(offset, limit);
            var totalProductsCount = await _productService.GetProductsTotalCountAsync();

            _pagingHelper.UpdateHttpHeadersWithPagingInformations(totalProductsCount, offset, limit);

            return _mapper.Map<List<GetProductResponse>>(products);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetProductResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(EmptyResponse))]
        public async Task<ActionResult<GetProductResponse>>GetProduct(int id)
        {
            throw new ArgumentException();
            var product = await _productService.GetProductAsync(id);
            if(product == null)
            {
                return NotFound(new EmptyResponse());
            }

            return Ok(product);
        }
    }
}
