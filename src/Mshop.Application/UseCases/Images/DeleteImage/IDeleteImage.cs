using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.DeleteImage
{
    public interface IDeleteImage : IRequestHandler<DeleteImageInPut, Result<ImageOutPut>>
    {
        Task<Result<ImageOutPut>> Handle(DeleteImageInPut request, CancellationToken cancellationToken);
    }
}
