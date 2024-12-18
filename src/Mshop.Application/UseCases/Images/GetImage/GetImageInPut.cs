using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.GetImage
{
    public class GetImageInPut : IRequest<Result<ImageOutPut>>
    {
        public GetImageInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
