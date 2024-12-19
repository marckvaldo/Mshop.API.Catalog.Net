using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.Core.Test.UseCase;
using useCase = Mshop.Application.UseCases.Product.CreateProducts;


namespace Mshop.Application.UseCases.Product.CreateProduct
{
    public class CreateProductTestFixture : UseCaseBaseFixture
    {
        public CreateProductTestFixture():base()
        {
    
        }

        protected CreateProductInPut Faker()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);

            return new useCase.CreateProductInPut
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Thumb = ImageFake64(),
                CategoryId = product.CategoryId,
                Stock = product.Stock,
                IsActive = product.IsActive,
            };
        }


        protected CreateProductInPut Faker(string description, string name, decimal price, string imagem, Guid categoryId, decimal stock, bool isActive = true)
        {
            return new CreateProductInPut
            {
                Name = name,
                Description = description,
                Price = price,
                Thumb = ImageFake64(),
                CategoryId = categoryId,
                Stock = stock,
                IsActive = isActive
            };
        }

        public static IEnumerable<object[]> GetCreateProductInPutInvalid()
        {
            yield return new object[] { GetDescriptionProductGreaterThan1000CharactersInvalid() };
            yield return new object[] { GetDescriptionProductLessThan10CharactersInvalid() };
            yield return new object[] { GetNameProductGreaterThan255CharactersInvalid() };
            yield return new object[] { GetNameProductLessThan3CharactersInvalid() };
        }

        protected static CreateProductInPut GetDescriptionProductGreaterThan1000CharactersInvalid()
        {
            string description = fakerStatic.Commerce.ProductDescription();
            while (description.Length < 1001)
            {
                description += fakerStatic.Commerce.ProductDescription();
            }


            return new CreateProductInPut
            {
                Name = fakerStatic.Commerce.ProductName(),
                Description = description,
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                CategoryId = Guid.NewGuid(),
                Stock = fakerStatic.Random.UInt(),
                IsActive = true,
                Thumb = ImageFake64()
            };

        }

        protected static CreateProductInPut GetDescriptionProductLessThan10CharactersInvalid()
        {
            string description = fakerStatic.Commerce.ProductDescription();
            description = description[..9];


            return new CreateProductInPut
            {
                Name = fakerStatic.Commerce.ProductName(),
                Description = description,
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                Stock = fakerStatic.Random.UInt(),
                IsActive = true
            };

        }

        protected static CreateProductInPut GetNameProductGreaterThan255CharactersInvalid()
        {
            string name = fakerStatic.Commerce.ProductName();
            while (name.Length < 255)
            {
                name += fakerStatic.Commerce.ProductName();
            }


            return new CreateProductInPut
            {
                Name = name,
                Description = fakerStatic.Commerce.ProductDescription(),
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                Stock = fakerStatic.Random.UInt(),
                IsActive = true
            };

        }

        protected static CreateProductInPut GetNameProductLessThan3CharactersInvalid()
        {
            string name = fakerStatic.Commerce.ProductDescription();
            name = name[..2];


            return new CreateProductInPut
            {
                Name = name,
                Description = fakerStatic.Commerce.ProductDescription(),
                Price = Convert.ToDecimal(fakerStatic.Commerce.Price()),
                Thumb = ImageFake64(),
                CategoryId = Guid.NewGuid(),
                Stock = fakerStatic.Random.UInt(),
                IsActive = true
            };

        }

    }
}
