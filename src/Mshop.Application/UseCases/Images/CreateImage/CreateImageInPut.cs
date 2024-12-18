using MediatR;
using Mshop.Application.Common;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.CreateImage
{
    public class CreateImageInPut : IRequest<Result<ListImageOutPut>>
    {
        public List<FileInputBase64>? Images {get; set; }

        public Guid ProductId { get; set; }
    }
}
