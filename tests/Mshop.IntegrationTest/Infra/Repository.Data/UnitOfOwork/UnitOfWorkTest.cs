using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.UnitOfWork;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Infra.Repository.Data.UnitOfOwork
{
    [Collection("Repository UnitOfWork")]
    [CollectionDefinition("Repository UnitOfWork", DisableParallelization = true)]
    public class UnitOfWorkTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly RepositoryDbContext _DbContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _publisher;
      
        public UnitOfWorkTest() : base()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _publisher = _serviceProvider.GetRequiredService<IDomainEventPublisher>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            _unitOfWork = new UnitOfWork(_DbContext, _publisher);
           
        }

        [Fact(DisplayName = nameof(Commit))]
        [Trait("Integration - Infra.Data", "UnitOfWork")]
        public async Task Commit()
        {
            var category = FakerCategory();
            var productExemple = FakerProduct(category);

            await _DbContext.AddAsync(productExemple);

            await _unitOfWork.CommitAsync(CancellationToken.None);

            var savedProduct = _DbContext.Products.AsNoTracking().First();
            Assert.Equal(savedProduct.Id, productExemple.Id);

        }

        public async void Dispose()
        {
            //await DeleteDataBase(_DbContext);
        }
    }
}
