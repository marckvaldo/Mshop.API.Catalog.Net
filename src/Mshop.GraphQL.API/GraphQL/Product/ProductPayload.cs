using Mshop.API.GraphQL.GraphQL.Category;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Domain.Entity;

namespace Mshop.API.GraphQL.GraphQL.Product
{
    public class ProductPayload : ProductModelOutPut
    {
        public List<string?> Images { get; set; }
        public ProductPayload(Guid id,
           string description,
           string name,
           decimal price,
           string? thumb,
           decimal stock,
           bool isActive,
           Guid categoryId,
           CategoryPayload category,
           bool isPromotion)
           : base(id, description, name, price, thumb, stock, isActive, categoryId,category, isPromotion) { Images = new List<string?>(); }

        public void AddImages(string images)
        {
            Images.Add(images);
        }




    }
}
