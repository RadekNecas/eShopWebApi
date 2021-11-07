using AutoMapper;
using eShopWebApi.Core.Entities.Product;
using eShopWebApi.ViewModels;

namespace eShopWebApi.MappingProfiles
{
    public class ProductProfiles : Profile
    {
        public ProductProfiles()
        {
            CreateMap<Product, GetProductResponse>();
        }
    }
}
