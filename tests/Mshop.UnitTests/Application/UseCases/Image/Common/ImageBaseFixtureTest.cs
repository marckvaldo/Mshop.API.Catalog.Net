using Mshop.Application.Common;
using Mshop.UnitTests.Common;
using System.Text;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Image.Common
{
    public class ImageBaseFixtureTest : BaseFixture
    {
        public DomainEntity.Image Faker(Guid productId)
        {
            return new DomainEntity.Image(faker.Image.LoremFlickrUrl(), productId);
        }

        public List<DomainEntity.Image> FakerImages(Guid productId, int quantity)
        {
            List<DomainEntity.Image> listImages = new List<DomainEntity.Image>();
            for (int i = 1; i <= quantity; i++)
                listImages.Add(Faker(productId));
            
            return listImages;
        }


        public FileInput ImageFaker()
        {
            return new FileInput("jpg", new MemoryStream(Encoding.ASCII.GetBytes(fakerStatic.Image.LoremFlickrUrl())));
        }


        public List<FileInput> ImageFakers(int quantity)
        {
            List<FileInput> listImage = new List<FileInput>();
            for (int i = 0; i <= quantity; i++)
                listImage.Add(ImageFaker());

            return listImage;
        }


        public List<FileInputBase64> ImageFakers64(int quantity)
        {
            var listImage = new List<FileInputBase64>();
            for (int i = 0; i <= quantity; i++)
                listImage.Add(ImageFake64());

            return listImage;
        }


        public DomainEntity.Product FakerProduct(DomainEntity.Category category)
        {
            var product = new DomainEntity.Product
                (
                     faker.Commerce.ProductName(),
                     faker.Commerce.ProductDescription(),
                    Convert.ToDecimal(faker.Commerce.Price()),
                    category.Id,
                    faker.Random.UInt(),
                    true
                );

            return product;
        }

        public DomainEntity.Category FakerCategory()
        {
            var category = new DomainEntity.Category
                (
                     faker.Commerce.Categories(1)[0],
                     true
                );

            return category;
        }
    }
}
