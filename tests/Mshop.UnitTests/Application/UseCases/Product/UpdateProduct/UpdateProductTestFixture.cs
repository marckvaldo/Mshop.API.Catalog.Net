using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.UnitTests.Common;
using System.Text;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.UpdateProduct;

namespace Mshop.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductTestFixture : BaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;
        public UpdateProductTestFixture() : base()
        {
            _categoryId = Guid.NewGuid();
            _id = Guid.NewGuid();
        }

        protected static FileInput ImageFake()
        {
            return new FileInput("jpg", new MemoryStream(Encoding.ASCII.GetBytes(fakerStatic.Image.LoremFlickrUrl())));
        }
       

        protected useCase.UpdateProductInPut ProductInPut()
        {
            return new useCase.UpdateProductInPut
            {
                Id = _id,
                Name = Faker().Name,
                Description = Faker().Description,
                Price = Faker().Price,
                Thumb = ImageFake64(),
                CategoryId = Faker().CategoryId,
                IsActive = Faker().IsActive
            };
            
        }

        protected ProductModelOutPut ProductModelOutPut()
        {
            return new ProductModelOutPut
            ( 
                _id,
                Faker().Description,
                Faker().Name,
                Faker().Price,
                Faker().Thumb.Path,
                Faker().Stock,
                Faker().IsActive,
                Faker().CategoryId
            );

        }

        protected DomainEntity.Product Faker()
        {
            DomainEntity.Product product = (new DomainEntity.Product
            (
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                Convert.ToDecimal(faker.Commerce.Price()),
                _categoryId,
                faker.Random.UInt(),
                true
            ));

            product.UpdateThumb(faker.Image.LoremFlickrUrl());
            return product;
        }

        protected DomainEntity.Category FakerCategory()
        {
            return new(faker.Commerce.Categories(1)[0]);
        }



        public static IEnumerable<object[]> GetUpdateProductInPutInvalid()
        {
            yield return new object[] { GetDescriptionProductGreaterThan1000CharactersInvalid() };
            yield return new object[] { GetDescriptionProductLessThan10CharactersInvalid() };
            yield return new object[] { GetNameProductGreaterThan255CharactersInvalid() };
            yield return new object[] { GetNameProductLessThan3CharactersInvalid() };
        }

        protected static useCase.UpdateProductInPut GetDescriptionProductGreaterThan1000CharactersInvalid()
        {
            string description = fakerStatic.Commerce.ProductDescription();
            while (description.Length < 1001)
            {
                description += fakerStatic.Commerce.ProductDescription();
            }


            return new useCase.UpdateProductInPut
            {
                Name = fakerStatic.Commerce.ProductName(),
                Description = description,
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

        }

        protected static useCase.UpdateProductInPut GetDescriptionProductLessThan10CharactersInvalid()
        {
            string description = fakerStatic.Commerce.ProductDescription();
            description = description[..9];


            return new useCase.UpdateProductInPut
            {
                Name = fakerStatic.Commerce.ProductName(),
                Description = description,
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

        }

        protected static useCase.UpdateProductInPut GetNameProductGreaterThan255CharactersInvalid()
        {
            string name = fakerStatic.Commerce.ProductName();
            while (name.Length < 255)
            {
                name += fakerStatic.Commerce.ProductName();
            }


            return new useCase.UpdateProductInPut
            {
                Name = name,
                Description = fakerStatic.Commerce.ProductDescription(),
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

        }

        protected static useCase.UpdateProductInPut GetNameProductLessThan3CharactersInvalid()
        {
            string name = fakerStatic.Commerce.ProductDescription();
            name = name[..2];


            return new useCase.UpdateProductInPut
            {
                Name = name,
                Description = fakerStatic.Commerce.ProductDescription(),
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

        }

    }
}
