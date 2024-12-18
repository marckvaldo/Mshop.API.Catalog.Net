using MediatR;
using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Product.UpdateThumb
{
    public class UpdateThumbInPut : IRequest<Result<ProductModelOutPut>>
    {
        public Guid Id { get; set; }
        public FileInputBase64 Thumb { get; set; }   
        
    }
}
