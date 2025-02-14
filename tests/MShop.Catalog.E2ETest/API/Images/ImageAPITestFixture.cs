using Mshop.Application.Common;
using Mshop.Application.UseCases.Images.CreateImage;
using Mshop.Catalog.E2ETests.Base;
using Mshop.Core.Test.Common;

namespace Mshop.Catalog.E2ETests.API.Images
{
    public class ImageAPITestFixture : BaseWebApplication
    {
        //private readonly Guid _productId;
        //protected readonly ImagePersistence _imagePersistence;
        //protected readonly ProductPersistence _productPersistence;
        //protected readonly CategoryPersistence _categoryPersistence;
        public ImageAPITestFixture() : base()
        { 
            //_productId = Guid.NewGuid();
            //_imagePersistence = new ImagePersistence(CreateDBContext());
            //_productPersistence = new ProductPersistence(CreateDBContext());  
            //_categoryPersistence = new CategoryPersistence(CreateDBContext());
        }
        
        public async Task<CreateImageInPut> FakeRequest(Guid productId, int quantidade = 3)
        {
            return new CreateImageInPut { Images = ListFakeImage64(quantidade), ProductId = productId };
        }

        public List<FileInputBase64> ListFakeImage64(int quantidade = 3)
        {
            var images = new List<FileInputBase64>();
            for (int i = 0; i < quantidade; i++)
                images.Add(FakeImage64());

            return images;
        }

        public FileInputBase64 FakeImage64()
        {
            return new FileInputBase64(FileFakerBase64.IMAGE64);
        }

        
        
        /*public BusinessEntity.Image FakeImage()
        {
            return new BusinessEntity.Image(faker.Image.LoremFlickrUrl(), _productId); ;
        }

        public List<BusinessEntity.Image> ListImage(int quantidade = 4)
        {
            var images = new List<BusinessEntity.Image>();
            for(var i = 0; i < quantidade; i++) 
                images.Add(FakeImage());
            return images;
        }*/
    }
}
