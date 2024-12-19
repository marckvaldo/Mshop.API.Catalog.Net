using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Test.UseCase;
using useCase = Mshop.Application.UseCases.Product.UpdateProduct;

namespace Mshop.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductTestFixture : UseCaseBaseFixture
    {
       
        public UpdateProductTestFixture() : base()
        {
           
        }

       
       

        protected useCase.UpdateProductInPut ProductInPut()
        {
            var category = FakerCategory();
            var products = FakerProduct(category);

            return new useCase.UpdateProductInPut
            {
                Id = products.Id,
                Name = products.Name,
                Description = products.Description,
                Price = products.Price,
                Thumb = ImageFake64(),
                CategoryId = products.CategoryId,
                IsActive = products.IsActive
            };
            
        }

        protected ProductModelOutPut ProductModelOutPut()
        {
            var category = FakerCategory();
            var products = FakerProduct(category);
            return new ProductModelOutPut
            (
                products.Id,
                products.Description,
                products.Name,
                products.Price,
                products.Thumb.Path,
                products.Stock,
                products.IsActive,
                products.CategoryId
            );

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
