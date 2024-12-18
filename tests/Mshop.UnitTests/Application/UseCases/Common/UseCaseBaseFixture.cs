using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.UnitTests.Application.UseCases.Common
{
    public class UseCaseBaseFixture : BaseFixture
    {
        public UseCaseBaseFixture():base() 
        { 

        }

        public DomainEntity.Category FakerCategory()
        {
            return new DomainEntity.Category(faker.Commerce.Categories(1)[0]);
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

        public List<DomainEntity.Image> FakerImage(Guid productId, int quantity = 3)
        {
            List<DomainEntity.Image> images = new();
            for (int i = 1; i <= quantity; i++)
                images.Add(new DomainEntity.Image(fakerStatic.Image.LoremFlickrUrl(), productId));

            return images;
        }
    }
}
