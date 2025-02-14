using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.Application.UseCases.Product.UpdateProduct;
using Mshop.Catalog.E2ETests.Base;
using Entity = Mshop.Domain.Entity;



namespace Mshop.Catalog.E2ETests.API.Cache.Product
{
    public class ProductAPITestFixture : BaseWebApplication
    {
        public ProductAPITestFixture() : base()
        {
        }

        public async Task<CreateProductInPut> RequestCreate(Entity.Category category)
        {
            var fakerProduct = FakerProduct(category);
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

        public async Task<UpdateProductInPut> RequestUpdate(Entity.Category Category)
        {
            var fakerProduct = FakerProduct(Category);
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
