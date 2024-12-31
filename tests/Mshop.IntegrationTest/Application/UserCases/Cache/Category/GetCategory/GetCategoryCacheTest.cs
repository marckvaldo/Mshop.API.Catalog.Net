using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Service;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Cache.Category.GetCategory;

namespace Mshop.IntegrationTests.Application.UserCases.Cache.Category.GetCategory
{
    [Collection("Get Category Collection")]
    [CollectionDefinition("Get Category Collection", DisableParallelization = true)]
    public class GetCategoryCacheTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBuildCacheCategory _buildCacheCategory;
        private readonly RepositoryDbContext _DbContext;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly StartIndex _startIndex;

        public GetCategoryCacheTest(): base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _buildCacheCategory = _serviceProvider.GetRequiredService<IBuildCacheCategory>();   
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();
        }

        [Fact(DisplayName = nameof(GetCategoryCache))]
        [Trait("Integration - Application.UseCase.Cache", "Category Use Case")]
        public async Task GetCategoryCache()
        {
            var categoryFake = FakerCategory();
            await _categoryRepository.Create(categoryFake, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var guid = categoryFake.Id;
            var useCase = new ApplicationUseCase.GetCategoryCache(_notification, _categoryCacheRepository, _categoryRepository, _buildCacheCategory);
            var outPut = await useCase.Handle(new ApplicationUseCase.GetCategoryCacheInPut(guid), CancellationToken.None);

            var categoryCache = _categoryCacheRepository.GetById(guid);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.NotNull(categoryCache);
            Assert.True(outPut.IsSuccess);
            Assert.Equal(guid, result.Id);
            Assert.Equal(result.Name, categoryFake.Name);
            Assert.Equal(result.IsActive, categoryFake.IsActive);

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProductCache))]
        [Trait("Integration - Application.UseCase.Cache", "Category Use Case")]
        public async Task SholdReturnErrorWhenCantGetProductCache()
        {
            var categoryFake = FakerCategory();
            await _categoryRepository.Create(categoryFake, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.GetCategoryCache(_notification, _categoryCacheRepository, _categoryRepository, _buildCacheCategory);

            //var outPut = async () => await useCase.Handle(new ApplicationUseCase.GetCategoryInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = await Assert.ThrowsAsync<ApplicationValidationException>(outPut);

            var outPut = await useCase.Handle(new ApplicationUseCase.GetCategoryCacheInPut(Guid.NewGuid()), CancellationToken.None);

            //Assert.Equal("Error", exception.Message);
            Assert.True(_notification.HasErrors());
            Assert.False(outPut.IsSuccess);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }

    }
}
