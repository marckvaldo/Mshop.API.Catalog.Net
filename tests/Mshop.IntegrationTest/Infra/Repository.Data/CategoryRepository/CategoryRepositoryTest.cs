using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mshop.Core.Data;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.UnitOfWork;
using Mshop.IntegrationTests.Common;
using DataRepository = Mshop.Infra.Data.Interface;

namespace Mshop.IntegrationTests.Infra.Repository.Data.CategoryRepository
{
    [Collection("Repository Category Collection")]
    [CollectionDefinition("Repository Category Collection", DisableParallelization = true)]
    public class CategoryRepositoryTest : IntegracaoBaseFixture
    {
        private readonly DataRepository.ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly RepositoryDbContext _DbContext;
        private readonly CategoryRepositoryPertsistence _persistenceCategory;
        

        public CategoryRepositoryTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            _persistenceCategory = new CategoryRepositoryPertsistence(_DbContext);
            
            DeleteDataBase(_DbContext,false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]

        public async Task CreateCategory()
        {
            var request = FakerCategory();
            await _categoryRepository.Create(request, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
            var newCategory = await _persistenceCategory.GetCategory(request.Id);

            Assert.NotNull(newCategory);
            Assert.Equal(newCategory.Name, request.Name);
            Assert.Equal(newCategory.IsActive, request.IsActive);
        }


        [Fact(DisplayName = nameof(GetByIdCategory))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]

        public async void GetByIdCategory()
        {
            var categoryFaker = FakerCategory();
            _persistenceCategory.Create(categoryFaker);

            var category = await _categoryRepository.GetById(categoryFaker.Id);

            Assert.NotNull(category);
            Assert.Equal(category.Name, categoryFaker.Name);
            Assert.Equal(category.IsActive, categoryFaker.IsActive);    
        }


        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]

        public async void UpdateProduct()
        {
            var categoryFaker = FakerCategories(3);
            await _persistenceCategory.CreateList(categoryFaker);

            var category = categoryFaker.FirstOrDefault();
            Assert.NotNull(category);
            category.Update(faker.Commerce.Categories(1)[0]);
            category.Deactive();

            await _categoryRepository.Update(category, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
            var categoryDb = await _persistenceCategory.GetCategory(category.Id);

            Assert.NotNull(categoryDb);            
            Assert.Equal(categoryDb.Name, category.Name);
            Assert.Equal(categoryDb.IsActive, category.IsActive);

        }


        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]

        public async Task DeleteCategory()
        {
            var quantity = 3;
            var categoryFaker = FakerCategories(quantity);
            await _persistenceCategory.CreateList(categoryFaker);

            var categoryDelete = categoryFaker.FirstOrDefault();    
            Assert.NotNull(categoryDelete);
            await _categoryRepository.DeleteById(categoryDelete, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var outPut = await _persistenceCategory.GetAllCategories();

            Assert.NotNull(outPut);
            Assert.Equal(quantity - 1, outPut.Count());
            Assert.Equal(0, outPut?.Where(x => x.Id == categoryDelete.Id).Count());
            
        }

        [Fact(DisplayName = nameof(FilterPaginated))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]

        public async Task FilterPaginated()
        {
            var quantity = 20;
            var perPage = 10;
            var categories = FakerCategories(quantity);
            await _persistenceCategory.CreateList(categories);

            var request = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Desc);

            var outPut = await _categoryRepository.FilterPaginated(request,CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut?.Itens);
            Assert.Equal(outPut.Total, quantity);
            Assert.Equal(outPut?.PerPage, perPage);
            Assert.Equal(outPut?.Itens.Count(), perPage);
            Assert.Equal(outPut?.CurrentPage, 1);

            foreach(var item in outPut?.Itens?.ToList())
            {
                var category = categories.Where(x => x.Id == item.Id).FirstOrDefault();
                Assert.NotNull(category);
                Assert.Equal(category.Name, item.Name);
                Assert.Equal(category.IsActive, item.IsActive);
            }

        }


        [Fact(DisplayName = nameof(SholdResultListEmptyFilterPaginated))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]
        public async Task SholdResultListEmptyFilterPaginated()
        {
            var perPage = 20;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _categoryRepository.FilterPaginated(input, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.True(outPut.Itens.Count == 0);
            Assert.True(outPut.Total == 0);
            Assert.Equal(input.PerPage, outPut.PerPage);
        }


        [Theory(DisplayName = nameof(SerachRestusPaginated))]
        [Trait("Integration - Infra.Data", "Category Repositorio")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(17, 3, 10, 0)]

        public async Task SerachRestusPaginated(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var categoryList = FakerCategories(quantityProduct);
            await _persistenceCategory.CreateList(categoryList);

            var input = new Core.Paginated.PaginatedInPut(page, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _categoryRepository.FilterPaginated(input, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Itens);
            Assert.True(outPut.Itens.Count == expectedQuantityItems);
            Assert.Equal(outPut.PerPage, perPage);
            Assert.True(outPut.Total == quantityProduct);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach (var item in outPut.Itens)
            {
                var category = categoryList.Where(x => x.Id == item.Id).FirstOrDefault();
                Assert.NotNull(category);
                Assert.Equal(category.Name, item.Name);
                Assert.Equal(category.IsActive, item.IsActive);
            }

        }

    }
}
