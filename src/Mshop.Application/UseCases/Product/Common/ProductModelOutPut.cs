using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using Busines = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Product.Common
{
    public class ProductModelOutPut : IModelOutPut
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Stock { get;  set; }

        public bool IsActive { get; set; }

        public Guid CategoryId { get; set; }

        public string? Thumb { get; set; }

        public CategoryModelOutPut Category { get; set; }   

        public bool IsPromotion { get; set; }
        public ProductModelOutPut(Guid id, 
            string description,
            string name, 
            decimal price,
            string? thumb,
            decimal stock, 
            bool isActive, 
            Guid categoryId,
            CategoryModelOutPut category = null,
            bool isPromotion = false)
        {
            Description = description;
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = isActive;
            CategoryId = categoryId;
            Id = id;
            Thumb = thumb;
            Category = category;
            IsPromotion = isPromotion;
        }

        public static ProductModelOutPut FromProduct(Busines.Product product)
        {
            return new ProductModelOutPut(
                product!.Id,
                product.Description,
                product.Name,
                product.Price,
                product.Thumb?.Path,
                product.Stock,
                product.IsActive,
                product.CategoryId);
        }
    }
}
