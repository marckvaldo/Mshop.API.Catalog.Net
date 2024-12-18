using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;
using System.ComponentModel.DataAnnotations;
using Entity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Category.GetCatetoryWithProducts
{
    public class GetCategoryWithProductsOutPut : IModelOutPut
    {
        public GetCategoryWithProductsOutPut(Guid id, string name, bool isActive, List<ProductModelOutPut> products)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
            Products = products;
        }

        public Guid Id { get; private set; }

        [Required(ErrorMessage = "O Campo {0} Obrigatório")]
        [StringLength(30, ErrorMessage = "O Campo {0} precisa ter no minimo {2} caracter e no maximo {1}", MinimumLength = 2)]
        public string Name { get; private set; }

        public bool IsActive { get; private set; }

        public List<ProductModelOutPut> Products { get; private set; }

        public static GetCategoryWithProductsOutPut FromCategory(Entity.Category category, List<ProductModelOutPut> products)
        {
            return new GetCategoryWithProductsOutPut(
                    category.Id,
                    category.Name,
                    category.IsActive,
                    products);
        }


    }
}
