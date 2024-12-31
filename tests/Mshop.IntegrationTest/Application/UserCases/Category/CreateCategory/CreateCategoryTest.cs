using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using ApplicationUseCase = Mshop.Application.UseCases.Category.CreateCategory;

namespace Mshop.IntegrationTests.Application.UserCases.Category.CreateCategory
{

    [Collection("Create Category Collection")]
    [CollectionDefinition("Create Category Collection", DisableParallelization = true)]
    public class CreateCategoryTest : CreateCategoryTestFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public CreateCategoryTest() : base()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]
        public async Task CreateCategory()
        {
            var request = FakerCreateCategoryInput();
            var useCase = new ApplicationUseCase.CreateCategory(
                _notification,
                _categoryRepository,
                _unitOfWork);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var categoryDB = await _categoryRepository.GetById(outPut.Data.Id);

            Assert.False(_notification.HasErrors());
            Assert.NotNull(outPut?.Data);
            Assert.Equal(outPut?.Data?.Name, categoryDB.Name);
            Assert.Equal(outPut?.Data?.IsActive, categoryDB.IsActive);
        }



        [Theory(DisplayName = nameof(ShoudReturErroWhenCreateCategoryWithInvalidName))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]
        [InlineData("nn")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("marckvaldo marckvlado marckvaldo markvaldo marckvaldo marckvaldo")]
        public async Task ShoudReturErroWhenCreateCategoryWithInvalidName(string name)
        {
            var request = FakerCreateCategoryInput();
            request.Name = name;

            var useCase = new ApplicationUseCase.CreateCategory(
                _notification,
                _categoryRepository,
                _unitOfWork);

            //var action = async () => await useCase.Handle(request, CancellationToken.None);
            //var exception = Assert.ThrowsAsync<EntityValidationException>(action);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            Assert.False(outPut.IsSuccess);
            Assert.True(_notification.HasErrors());
        }


        public void Dispose()
        {

        }
    }
}
