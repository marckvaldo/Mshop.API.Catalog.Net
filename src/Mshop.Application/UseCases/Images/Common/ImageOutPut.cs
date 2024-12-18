using Mshop.Core.DomainObject;

namespace Mshop.Application.UseCases.Images.Common
{
    public class ImageOutPut: IModelOutPut
    {
        public ImageOutPut(Guid productId, ImageModelOutPut image)
        {
            ProductId = productId;
            Image = image;
        }

        public Guid ProductId { get; set; }

        public ImageModelOutPut Image { get; set; }

    }
}
