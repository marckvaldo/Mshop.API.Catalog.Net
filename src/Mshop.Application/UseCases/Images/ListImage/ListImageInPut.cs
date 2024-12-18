using MediatR;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.ListImage
{
    public class ListImageInPut : IRequest<Result<ListImageOutPut>>
    {
        public ListImageInPut(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
