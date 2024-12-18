using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.Respository;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;

namespace Mshop.Application.Service
{
    public class BuildCacheProduct : IBuildCacheProduct
    {
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;

        public BuildCacheProduct(IProductCacheRepository productCacheRepository, IProductRepository productRepository)
        {
            _productCacheRepository = productCacheRepository;
            _productRepository = productRepository;
        }

        public async void Handle()
        {
            var expirationDate = DateTime.UtcNow.AddHours(1);

            var products = await _productRepository.GetProductAll(OnlyActive:true);

            foreach (var product in products)
            {
                await _productCacheRepository.AddProduct(product, expirationDate, CancellationToken.None);
            }
        }
    }
}
