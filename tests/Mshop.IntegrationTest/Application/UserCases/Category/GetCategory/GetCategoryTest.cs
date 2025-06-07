using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Category.GetCategory;

namespace Mshop.IntegrationTests.Application.UserCases.Category.GetCategory
{
    [Collection("Get Category Collection")]
    [CollectionDefinition("Get Category Collection", DisableParallelization = true)]
    public class GetCategoryTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public GetCategoryTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]
        public async Task GetCategory()
        {
            var categoryFake = FakerCategory();
            await _categoryRepository.Create(categoryFake,CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var guid = categoryFake.Id;
            var useCase = new ApplicationUseCase.GetCategory(_notification, _categoryRepository);
            var outPut = await useCase.Handle(new ApplicationUseCase.GetCategoryInPut(guid), CancellationToken.None);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(result.Name, categoryFake.Name);
            Assert.Equal(result.IsActive, categoryFake.IsActive);

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProduct))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]
        public async Task SholdReturnErrorWhenCantGetProduct()
        {
            var categoryFake = FakerCategory();
            await _categoryRepository.Create(categoryFake,CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.GetCategory(_notification,_categoryRepository);

            //var outPut = async () => await useCase.BuildCache(new ApplicationUseCase.GetCategoryInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = await Assert.ThrowsAsync<ApplicationValidationException>(outPut);

            var outPut = await useCase.Handle(new ApplicationUseCase.GetCategoryInPut(Guid.NewGuid()), CancellationToken.None);

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
