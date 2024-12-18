using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.ListImage
{
    public interface IListImage : IRequestHandler<ListImageInPut, Result<ListImageOutPut>>
    {
        Task<Result<ListImageOutPut>> Handle (ListImageInPut request, CancellationToken cancellation);
    }
}
