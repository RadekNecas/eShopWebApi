using eShopWebApi.Core.Entities.Product;
using System.ComponentModel.DataAnnotations;

namespace eShopWebApi.ViewModels
{
    public class GetProductResponse
    {
        public int Id { get; set; }
        
        [StringLength(Product.MaxNameLength)]
        public string Name { get; set; }
        
        [StringLength(Product.MaxImgUriLength)]
        public string ImgUri { get; set; }
        
        public decimal Price { get; set; }
        
        [StringLength(Product.MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
