using MediatR;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using System.ComponentModel.DataAnnotations;

namespace Mshop.Application.UseCases.Category.CreateCategory
{
    public class CreateCategoryInPut: IRequest<Result<CategoryModelOutPut>>
    {
       
        [Required(ErrorMessage = "O Campo {0} Obrigatório")]
        [StringLength(30, ErrorMessage = "O Campo {0} precisa ter no minimo {2} caracter e no maximo {1}", MinimumLength = 3)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
