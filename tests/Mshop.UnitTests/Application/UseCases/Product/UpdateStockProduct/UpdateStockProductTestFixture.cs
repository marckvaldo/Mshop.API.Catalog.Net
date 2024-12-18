using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.UpdateStockProduct;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public class UpdateStockProductTestFixture : BaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;

        public UpdateStockProductTestFixture() 
        {
            _categoryId = Guid.NewGuid();
            _id = Guid.NewGuid();
        }

        protected DomainEntity.Product Faker()
        {
            return (new DomainEntity.Product
            (
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                Convert.ToDecimal(faker.Commerce.Price()),
                _categoryId,
                faker.Random.UInt(),
                true
            ));
        }

        protected useCase.UpdateStockProductInPut UpdateStockProductInPut()
        {
            return new useCase.UpdateStockProductInPut
            {
                Id = _id,
                Stock = Faker().Stock
            };

        }
    }
}
