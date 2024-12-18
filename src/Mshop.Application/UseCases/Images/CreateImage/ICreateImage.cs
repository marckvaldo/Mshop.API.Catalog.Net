using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.CreateImage
{
    public interface ICreateImage : IRequestHandler<CreateImageInPut, Result<ListImageOutPut>>
    {
        Task<Result<ListImageOutPut>> Handle(CreateImageInPut request, CancellationToken cancellation);
    }
}
