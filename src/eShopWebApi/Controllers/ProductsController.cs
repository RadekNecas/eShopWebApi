using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public ProductsController(IProductService productService)
        {
            _productService = Guard.ReturnIfChecked(productService, nameof(productService));
        }


        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ICollection<GetProductVM>> GetProducts()
        {
            var products = await _productService.GetProducts();
            return products.Select(p => new GetProductVM
            {
                Id = p.Id,
                Name = p.Name,
                ImgUri = p.ImgUri,
                Price = p.Price,
                Description = p.Description
            }).ToList();
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ICollection<GetProductVM>> GetProductsPaginated()
        {
            return Array.Empty<GetProductVM>();
        }
    }
}
