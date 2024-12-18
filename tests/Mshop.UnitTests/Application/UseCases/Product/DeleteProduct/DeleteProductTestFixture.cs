using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;


namespace Mshop.Application.UseCases.Product.DeleteProduct
{
    public abstract class DeleteProductTestFixture : BaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;
        public DeleteProductTestFixture() : base()
        {
            _categoryId = Guid.NewGuid();
            _id = Guid.NewGuid();
        }

        protected DomainEntity.Product Faker()
        {
            var product = (new DomainEntity.Product
            (
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                Convert.ToDecimal(faker.Commerce.Price()),
                _categoryId,
                _id,
                faker.Random.UInt(),
                true
            ));
            return product;
        }
    }
}
