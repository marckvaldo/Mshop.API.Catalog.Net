using Mshop.Application.Common;
using Mshop.Core.Test.Common;
using System.Text;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Core.Test.UseCase
{
    public class UseCaseBaseFixture : BaseFixture
    {
        public UseCaseBaseFixture():base() 
        { 

        }

        public List<DomainEntity.Category> FakerCategories(int quantity)
        {
            List<DomainEntity.Category> listCategory = new List<DomainEntity.Category>();
            for (int i = 1; i <= quantity; i++)
                listCategory.Add(FakerCategory());

            return listCategory;
        }

        public DomainEntity.Product FakerProduct(DomainEntity.Category category)
        {
            var product = (new DomainEntity.Product
            (
                faker.Commerce.ProductDescription(),
                faker.Commerce.ProductName(),
                Convert.ToDecimal(faker.Commerce.Price()),
                category.Id,
                faker.Random.UInt(),
                true
            ));

            product.UpdateThumb(faker.Image.LoremFlickrUrl());
            product.AddCategory(category);
            return product;
        }

        public List<DomainEntity.Product> FakerProducts(int quantity, DomainEntity.Category category)
        {
            List<DomainEntity.Product> listProduct = new List<DomainEntity.Product>();
            for (int i = 1; i <= quantity; i++)
                listProduct.Add(FakerProduct(category));

            return listProduct;
        }

        public static FileInput FakerImageFileInput()
        {
            return new FileInput("jpg", new MemoryStream(Encoding.ASCII.GetBytes(fakerStatic.Image.LoremFlickrUrl())));
        }

    }
}
