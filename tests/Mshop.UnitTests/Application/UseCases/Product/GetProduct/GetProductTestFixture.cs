using Mshop.UnitTests.Application.UseCases.Product.Common;
using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.UnitTests.Application.UseCases.Product.GetProduct
{
    public class GetProductTestFixture: ProductBaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;
        public GetProductTestFixture() : base()
        {
            _categoryId = Guid.NewGuid();
            _id = Guid.NewGuid();
        }

       

    }
}
