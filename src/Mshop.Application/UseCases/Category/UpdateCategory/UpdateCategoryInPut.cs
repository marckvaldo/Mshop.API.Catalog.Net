using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using System.ComponentModel.DataAnnotations;

namespace Mshop.Application.UseCases.Category.UpdateCategory
{
    public class UpdateCategoryInPut: IRequest<Result<CategoryModelOutPut>>
    {
        public Guid Id { get; set; } 

        [Required(ErrorMessage = "O Campo {0} Obrigatório")]
        [StringLength(30, ErrorMessage = "O Campo {0} precisa ter no minimo {2} caracter e no maximo {1}", MinimumLength = 2)]
        public string Name { get; set; }

        public bool IsActive { get;  set; }
    }
}
