using Mshop.Core.Test.UseCase;
using useCase = Mshop.Application.UseCases.Product.UpdateStockProduct;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public class UpdateStockProductTestFixture : UseCaseBaseFixture
    {
        private readonly Guid _id;

        public UpdateStockProductTestFixture() 
        {          
            _id = Guid.NewGuid();
        }

      
        protected useCase.UpdateStockProductInPut UpdateStockProductInPut(decimal stock)
        {
            return new useCase.UpdateStockProductInPut
            {
                Id = _id,
                Stock = stock
            };

        }
    }
}
