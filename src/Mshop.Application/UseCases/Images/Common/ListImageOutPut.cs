using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.Common
{
    public class ListImageOutPut: IModelOutPut
    {
        public ListImageOutPut(Guid productId, List<ImageModelOutPut> images)
        {
            ProductId = productId;
            Images = images;
        }

        public Guid ProductId { get; set; }

        public List<ImageModelOutPut> Images { get; set; }
    }
}
