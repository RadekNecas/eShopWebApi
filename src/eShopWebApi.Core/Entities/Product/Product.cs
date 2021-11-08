using eShopWebApi.Core.Tools;
using System.Text.Json.Serialization;

namespace eShopWebApi.Core.Entities.Product
{
    public class Product: BaseEntity, IAggregateRoot
    {
        public Product()
        {
        }

        public Product(string name, string imgUri, decimal price, string description)
        {
            Name = Guard.ReturnIfNotNullOrEmpty(name, nameof(name)).Trim();
            ImgUri = Guard.ReturnIfNotNullOrEmpty(imgUri, nameof(imgUri)).Trim();
            Price = Guard.ReturnIfPositiveNumber(price, nameof(price));
            Description = description?.Trim();
        }

        [JsonInclude]
        public string Name { get; private set; }

        [JsonInclude]
        public string ImgUri { get; private set; }

        [JsonInclude]
        public decimal Price { get; private set; }

        [JsonInclude]
        public string Description { get; private set; }

        public void SetDescription(string newDescription)
        {
            Description = newDescription?.Trim();
        }
    }
}
