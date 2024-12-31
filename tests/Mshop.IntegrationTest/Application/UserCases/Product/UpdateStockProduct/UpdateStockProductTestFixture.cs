using Mshop.Application.UseCases.Product.UpdateStockProduct;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Product.UpdateStockProduct;

namespace Mshop.IntegrationTests.Application.UserCases.Product.UpdateStockProduct
{
    public class UpdateStockProductTestFixture : IntegracaoBaseFixture
    {
        public UpdateStockProductTestFixture() : base()
        {
           
        }

        protected UpdateStockProductInPut FakerUpdateStockProdjctInPut(Guid id)
        {
            var category = FakerCategory();
            var FakerProduto = FakerProduct(category);
            var product = (new ApplicationUseCase.UpdateStockProductInPut
            {
                Stock = FakerProduto.Stock,
                Id = id
            });
            return product;
        }

    }
}
