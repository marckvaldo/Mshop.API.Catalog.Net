using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.Application.UseCases.Product.UpdateProduct;
using MShop.Catalog.E2ETests.Base;


namespace MShop.Catalog.E2ETests.API.Product
{
    public class ProductAPITestFixture : BaseWebApplication
    {
        public ProductAPITestFixture() : base()
        {
        }

        public async Task<CreateProductInPut> RequestCreate()
        {
            var fakerProduct = FakerProduct(FakerCategory());
            return new CreateProductInPut
            {
                Name = fakerProduct.Name,
                CategoryId = fakerProduct.CategoryId,
                Thumb = ImageFake64(),
                IsActive = true,
                Description = fakerProduct.Description,
                Price = fakerProduct.Price,
                Stock = fakerProduct.Stock
            };
        }

        public async Task<UpdateProductInPut> RequestUpdate()
        {
            var fakerProduct = FakerProduct(FakerCategory());
            return new UpdateProductInPut
            {
                Name = fakerProduct.Name,
                CategoryId = fakerProduct.CategoryId,
                Thumb = ImageFake64(),
                IsActive = true,
                Description = fakerProduct.Description,
                Price = fakerProduct.Price,
                Id = fakerProduct.Id
            };
        }
    }
}
