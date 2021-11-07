using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace eShopWebApi.Infrastructure.Data.Initialization
{
    public class ProductsDataJsonProvider : InitDataProvider<Product>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileReader _fileReader;
        private readonly ILogger<ProductsDataJsonProvider> _logger;

        private string PathToJsonFile => _configuration["eShopWebApi:PathToInitProductsJsonFile"];

        public ProductsDataJsonProvider(IConfiguration configuration, IFileReader fileReader, 
            ILogger<ProductsDataJsonProvider> logger)
        {
            _configuration = Guard.ReturnIfChecked(configuration, nameof(configuration));
            _fileReader = Guard.ReturnIfChecked(fileReader, nameof(fileReader));
            _logger = Guard.ReturnIfChecked(logger, nameof(logger));
        }

        public IEnumerable<Product> GetData()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonProducts = _fileReader.ReadAllFile(PathToJsonFile);
            _logger.LogInformation("Loaded products JSON data from file {pathToJsonFile}", PathToJsonFile);
            _logger.LogDebug("Loaded JSON data: {data}", jsonProducts);
            return string.IsNullOrEmpty(jsonProducts) 
                    ? Enumerable.Empty<Product>()
                    : JsonSerializer.Deserialize<List<Product>>(jsonProducts, options);
        }
    }
}
