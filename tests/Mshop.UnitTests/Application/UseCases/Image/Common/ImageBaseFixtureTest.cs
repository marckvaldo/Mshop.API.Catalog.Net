using Mshop.Application.Common;
using Mshop.Core.Test.UseCase;

namespace Mshop.Application.UseCases.Image.Common
{
    public class ImageBaseFixtureTest : UseCaseBaseFixture
    {
        public ImageBaseFixtureTest()
        {
        }

       public List<FileInput> ImageFakersFileInput(int quantity)
        {
            List<FileInput> listImage = new List<FileInput>();
            for (int i = 0; i <= quantity; i++)
                listImage.Add(FakerImageFileInput());

            return listImage;
        }


        public List<FileInputBase64> ImageFakers64(int quantity)
        {
            var listImage = new List<FileInputBase64>();
            for (int i = 0; i <= quantity; i++)
                listImage.Add(ImageFake64());

            return listImage;
        }

    }
}
