using Mshop.Application.Common;
using Mshop.Application.UseCases.Image.Common;
using Mshop.Application.UseCases.Images.CreateImage;

namespace Mshop.Application.UseCases.Image.CreateImage
{
    public class CreateImageTestFixture : ImageBaseFixtureTest
    {
        public CreateImageInPut FakerRequest(Guid productId, List<FileInputBase64> images)
        {
            return new CreateImageInPut
            {
                Images = images,
                ProductId = productId
            };
        }

        

        
    }
}
