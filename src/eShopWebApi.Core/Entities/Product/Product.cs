using eShopWebApi.Core.Tools;
using System;
using System.Text.Json.Serialization;

namespace eShopWebApi.Core.Entities.Product
{
    public class Product: BaseEntity, IAggregateRoot
    {
        // Proper length limitation should be discussed with customer or domain expert.
        public const int MaxDescriptionLength = 300;
        public const int MaxImgUriLength = 2048;
        public const int MaxNameLength = 150;

        public Product()
        {
        }

        public Product(string name, string imgUri, decimal price, string description)
        {
            Name = Guard.ReturnIfNotNullStringLengthVerified(name, MaxNameLength, nameof(name)).Trim();
            ImgUri = Guard.ReturnIfNotNullStringLengthVerified(imgUri, MaxImgUriLength, nameof(imgUri)).Trim();
            Price = Guard.ReturnIfPositiveNumber(price, nameof(price));
            SetDescription(description);
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
            Description = Guard.ReturnIfStringLengthVerified(newDescription, MaxDescriptionLength, nameof(newDescription))?.Trim();
        }
    }
}
