using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.DeleteImage
{
    public class DeleteImageInPut : IRequest<Result<ImageOutPut>>
    {
        public DeleteImageInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
