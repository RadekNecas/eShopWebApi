using eShopWebApi.Core.Entities.Product;
using System.ComponentModel.DataAnnotations;

namespace eShopWebApi.ViewModels
{
    public class UpdateProductRequest
    {
        [StringLength(Product.MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
