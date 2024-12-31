using Mshop.Application.Common;
using Mshop.Application.UseCases.Images.CreateImage;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Images.CreateImage
{
    public class CreateImageTestFixture : IntegracaoBaseFixture
    {
        public CreateImageInPut FakerCreateImageInPut(Guid productId, int quantity = 3) 
        {
            return new CreateImageInPut
            {
                Images = FakeFileInputList64(quantity),
                ProductId = productId
            }; 
        }

        public List<FileInputBase64> FakeFileInputList64(int quantity = 3)
        {
            List<FileInputBase64> list = new List<FileInputBase64>();
            for (int i = 0; i < quantity; i++)
                list.Add(ImageFake64());

            return list;
        }
    }
}
