﻿using MediatR;
using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;
using System.ComponentModel.DataAnnotations;

namespace Mshop.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductInPut : IRequest<Result<ProductModelOutPut>>
    {
        //[Required(ErrorMessage = "O Campo {0} Obrigatório")]
        public Guid Id { get; set; }
    
        //[Required(ErrorMessage = "O Campo {0} Obrigatório")]
        //[StringLength(1000, ErrorMessage = "O Campo {0} precisa ter no minimo {2} caracter e no maximo {1}", MinimumLength = 2)]
        public string Description { get; set; }

        //[Required(ErrorMessage = "O Campo {0} Obrigatório")]
        //[StringLength(100, ErrorMessage = "O Campo {0} precisa ter no minimo {2} caracter e no maximo {1}", MinimumLength = 2)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public FileInputBase64? Thumb { get; set; }

        public bool IsActive { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsPromotion { get; set; }
    }
}