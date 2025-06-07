using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using ApplicationUseCase = Mshop.Application.UseCases.Category.UpdateCategory;

namespace Mshop.IntegrationTests.Application.UserCases.Category.UpdateCategory
{
    [Collection("Update Category Collection")]
    [CollectionDefinition("Update Category Collection", DisableParallelization = true)]
    public class UpdateCategoryTest : UpdateCategoryTestFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public UpdateCategoryTest() : base()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]

        public async Task UpdateCategory()
        {

            var category = FakerCategory();
            var request = FakerUpdateCategoryInput();
            await _categoryRepository.Create(category,CancellationToken.None);
            await _unitOfWork.CommitAsync();
            request.Id = category.Id;

            var useCase = new ApplicationUseCase.UpdateCategory(
                _categoryRepository, 
                _notification,
                _unitOfWork);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var categoryDb = await _categoryRepository.GetById(category.Id);
            var categoryCache = await _categoryCacheRepository.GetById(category.Id);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.NotNull(categoryDb);
            Assert.NotEmpty(result.Name);
            Assert.Equal(result.Name, categoryDb.Name);
            Assert.Equal(result.IsActive, categoryDb.IsActive);
            Assert.Equal(result.Id, categoryDb.Id);
            Assert.Equal(categoryDb.Name, categoryCache.Name);
            Assert.Equal(categoryDb.IsActive, categoryCache.IsActive);
            Assert.Equal(categoryDb.Id, categoryCache.Id);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
