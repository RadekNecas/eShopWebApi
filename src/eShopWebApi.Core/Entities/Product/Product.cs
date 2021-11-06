namespace eShopWebApi.Core.Entities.Product
{
    public class Product: BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string ImgUri { get; private set; }
        public decimal Price { get; private set; }
        public string Description { get; private set; }
    }
}
