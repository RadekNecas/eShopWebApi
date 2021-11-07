using System.Text.Json.Serialization;

namespace eShopWebApi.Core.Entities.Product
{
    public class Product: BaseEntity, IAggregateRoot
    {
        [JsonInclude]
        public string Name { get; private set; }

        [JsonInclude]
        public string ImgUri { get; private set; }

        [JsonInclude]
        public decimal Price { get; private set; }

        [JsonInclude]
        public string Description { get; private set; }
    }
}
