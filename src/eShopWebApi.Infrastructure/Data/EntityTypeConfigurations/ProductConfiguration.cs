using eShopWebApi.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopWebApi.Infrastructure.Data.EntityTypeConfigurations
{
    /// <summary>
    /// Max Length of text fields is just a simple estimate. 
    /// Correct field limitations would be discussed with domain expert.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.ImgUri)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.Description)
                .IsRequired(false)
                .HasMaxLength(300);
        }
    }
}
