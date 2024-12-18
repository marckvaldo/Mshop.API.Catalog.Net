using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.GetImage
{
    public interface IGetImage : IRequestHandler<GetImageInPut, Result<ImageOutPut>>
    {
        Task<Result<ImageOutPut>> Handle(GetImageInPut request, CancellationToken cancellation);
    }
}
