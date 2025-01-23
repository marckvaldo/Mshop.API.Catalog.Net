using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using MShop.Catalog.E2ETest.Base;
using MShop.Catalog.E2ETests.API.Common;

namespace MShop.Catalog.E2ETests.API.Category
{
    public class CategoryAPITest : CategoryAPITestFixture
    {
        private ICategoryRepository _categoryRepository;
        private ICategoryCacheRepository _categoryCacheRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        public CategoryAPITest() : base()
        {
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();

            DeleteDataBase(_DbContext,false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("EndToEnd/API", "Category - Endpoints")]

        public async Task CreateCategory()
        {
            var request = RequestCreate();
            var (response, outPut) = await _apiClient.Post<CustomResponse<CategoryModelOutPut>>(Configuration.URL_API_CATEGORY, request);

            Assert.NotNull(request);
            Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Name, request.Name);
            Assert.Equal(outPut.Data.IsActive, request.IsActive);

            var dbCategory = (await _categoryRepository.Filter(c=>c.Id == outPut.Data.Id)).First();

            Assert.NotNull(dbCategory);
            Assert.Equal(dbCategory.Name, request.Name);
            Assert.Equal(dbCategory.IsActive, request.IsActive);

        }


        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("EndToEnd/API", "Category - Endpoints")]

        public async Task UpdateCategory()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var request = FakerCategory();
            request.Id = category.Id;

            var (response, outPut) = await _apiClient.Put<CustomResponse<CategoryModelOutPut>>(
                $"{Configuration.URL_API_CATEGORY}{category.Id}",
                request);

            var categoryDb = (await _categoryRepository.Filter(c => c.Id == outPut.Data.Id)).First();

            Assert.NotNull(categoryDb);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(outPut.Data.Id, categoryDb.Id);
            Assert.Equal(outPut.Data.Name, categoryDb.Name);
            Assert.Equal(outPut.Data.IsActive, categoryDb.IsActive);

        }

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("EndToEnd/API", "Category - Endpoints")]

        public async Task DeleteCategory()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var request = FakerCategory();
            request.Id = category.Id;

            var (response, outPut) = await _apiClient.Delete<CustomResponse<CategoryModelOutPut>>($"{Configuration.URL_API_CATEGORY}{request.Id}");

            var categoryDb = (await _categoryRepository.Filter(c => c.Id == outPut.Data.Id)).FirstOrDefault();

            Assert.Null(categoryDb);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = nameof(GetCategoryById))]
        [Trait("EndToEnd/API", "Category - Endpoints")]

        public async Task GetCategoryById()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();


            var (response, outPut) = await _apiClient.Get<CustomResponse<CategoryModelOutPut>>($"{Configuration.URL_API_CATEGORY}{category.Id}");

            var categoryDb = (await _categoryRepository.Filter(c => c.Id == outPut.Data.Id)).First();

            Assert.NotNull(categoryDb);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(outPut.Data.Name, categoryDb.Name);
            Assert.Equal(outPut.Data.IsActive, categoryDb.IsActive);

        }

        [Fact(DisplayName = nameof(ListCategory))]
        [Trait("EndToEnd/API", "Category - Endpoints")]

        public async Task ListCategory()
        {
            var qtdCategory = 20;
            var perPager = 10;
            var page = 1;
            var categories = FakerCategories(qtdCategory);

            foreach (var category in categories)
            {
                await _categoryRepository.Create(category, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var query = new ListCategoryInPut(page, perPager, "", "", SearchOrder.Desc);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListCategoryOutPut>>($"{Configuration.URL_API_CATEGORY}list-category", query);

            var categoryDb = (await _categoryRepository.Filter(c => c.Name != null)).ToList();

            Assert.Equal(20, categoryDb.Count);
            Assert.NotNull(categories);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(outPut.Data.Itens);
            Assert.Equal(perPager, outPut.Data.PerPage);
            Assert.Equal(page, outPut.Data.Page);
            Assert.Equal(qtdCategory, outPut.Data.Total);

            foreach (var item in outPut.Data.Itens)
            {
                var category = (await _categoryRepository.Filter(c=>c.Id == item.Id)).First();
                Assert.NotNull(category);
                Assert.Equal(category.Name, item.Name);
                Assert.Equal(category.IsActive, item.IsActive);
            }

        }

    }
}
