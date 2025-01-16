using Microsoft.Extensions.DependencyInjection;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.Respository;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using System;

namespace Mshop.Application.Service
{
    public class BuildCacheProduct : IBuildCacheProduct
    {

       private IServiceProvider _serviceProvider;

        public BuildCacheProduct(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task Handle()
        {
            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var productCacheRepository = scope.ServiceProvider.GetRequiredService<IProductCacheRepository>();


            var expirationDate = DateTime.UtcNow.AddHours(1);

            var products = await productRepository.GetProductAll(OnlyActive:true);

            foreach (var product in products)
            {
                await productCacheRepository.Create(product, expirationDate, CancellationToken.None);
            }
        }
    }
}
