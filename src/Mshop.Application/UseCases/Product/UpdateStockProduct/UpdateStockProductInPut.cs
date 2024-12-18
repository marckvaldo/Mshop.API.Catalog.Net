using MediatR;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;
using System.ComponentModel.DataAnnotations;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public class UpdateStockProductInPut : IRequest<Result<ProductModelOutPut>>
    {
        [Required(ErrorMessage = "O Campo {0} Obrigatório")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O Campo {0} Obrigatório")]
        public decimal Stock { get; set; }
    }
}
