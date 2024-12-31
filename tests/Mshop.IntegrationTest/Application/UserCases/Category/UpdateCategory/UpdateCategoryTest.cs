using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
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
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public UpdateCategoryTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();            
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

             //await _categoryPersistence.Create(category);

            var useCase = new ApplicationUseCase.UpdateCategory(
                _categoryRepository, 
                _notification,
                _unitOfWork);
            var outPut = await useCase.Handle(request, CancellationToken.None);

            var categoryDb = await _categoryRepository.GetById(category.Id);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.NotNull(categoryDb);
            Assert.Equal(result.Name, categoryDb.Name);
            Assert.NotEmpty(result.Name);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
