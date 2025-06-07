using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Category.DeleteCategory;

namespace Mshop.IntegrationTests.Application.UserCases.Category.DeleteCategory
{

    [Collection("Delete Category Collection")]
    [CollectionDefinition("Delete Category Collection", DisableParallelization = true)]
    public class DeleteCategoryTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;


        public DeleteCategoryTest() : base()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]

        public async Task DeleteCategory()
        {
            var categorys = FakerCategories(10);

            foreach(var item in categorys)
            {
                await _categoryRepository.Create(item, CancellationToken.None);
                await _categoryCacheRepository.Create(item, DateTime.Now.AddMinutes(5), CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();
            


            var category = categorys.FirstOrDefault();

            var useCase = new ApplicationUseCase.DeleteCategory(
                _categoryRepository,
                _productRepository,
                _notification,
                _unitOfWork);

            await useCase.Handle(new ApplicationUseCase.DeleteCategoryInPut(category.Id), CancellationToken.None);

            var categoryDB = await _categoryRepository.GetById(category.Id);
            var categoryCache = await _categoryCacheRepository.GetById(category.Id);

            Assert.Null(categoryDB);
            Assert.Null(categoryCache);
            Assert.False(_notification.HasErrors());

        }


        [Fact(DisplayName = nameof(ShoudRetunrErrorWhenDeleteCategoryThatThereAreProdutcs))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]

        public async Task ShoudRetunrErrorWhenDeleteCategoryThatThereAreProdutcs()
        {
            var categorys = FakerCategories(10);
            foreach (var item in categorys)
            {
                await _categoryRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var category = categorys.FirstOrDefault();
            Assert.NotNull(category);

            var products = FakerProducts(10, category);
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();


            var useCase = new ApplicationUseCase.DeleteCategory(
                _categoryRepository,
                _productRepository,
                _notification,
                _unitOfWork);

            //var action = async () => await useCase.BuildCache(new ApplicationUseCase.DeleteCategoryInPut(category.Id), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<ApplicationValidationException>(action);

            await useCase.Handle(new ApplicationUseCase.DeleteCategoryInPut(category.Id), CancellationToken.None);

            var categoryDB = await _categoryRepository.GetById(category.Id);
            var productsDB = await _productRepository.Filter(x => x.CategoryId == category.Id);

            Assert.True(_notification.HasErrors());
            Assert.NotNull(categoryDB);
            Assert.True(productsDB.Count() == 10);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}


