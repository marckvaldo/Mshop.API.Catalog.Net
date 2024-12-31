using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Application.UseCases.Category.DeleteCategory;
using Mshop.Application.UseCases.Category.GetCategory;
using Mshop.Application.UseCases.Category.GetCatetoryWithProducts;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Application.UseCases.Category.UpdateCategory;
using Mshop.Application.UseCases.GetCatetoryWithProducts.GetCatetory;
using Mshop.Core.Enum.Paginated;
using Core = Mshop.Core.Message;


namespace Mshop.API.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : MainController
    {
        /*private readonly IGetCategory _getCategory;
        private readonly IListCategory _listCategory;
        private readonly ICreateCategory _createCategory;
        private readonly IUpdateCategory _updateCategory;
        private readonly IDeleteCategory _deleteCategory;
        private readonly IGetCategoryWithProducts _getCatetoryWithProducts;*/

        private readonly IMediator _mediator;
        public CategoryController(
            /*IGetCategory getCategory,
            IListCategory listCategory,
            ICreateCategory createCategory,
            IUpdateCategory updateCategory,
            IDeleteCategory deleteCategory,
            IGetCategoryWithProducts getCatetoryWithProducts,*/
            IMediator mediator,
            Core.Message.INotification notification) : base(notification)
        {
            /*_getCategory = getCategory;
            _listCategory = listCategory;
            _createCategory = createCategory;
            _updateCategory = updateCategory;
            _deleteCategory = deleteCategory;  
            _getCatetoryWithProducts = getCatetoryWithProducts; */
            _mediator = mediator;
        }

        [HttpGet("{Id:Guid}")]
        public async Task<ActionResult<CategoryModelOutPut>> Category(Guid Id, CancellationToken cancellation)
        {
                return CustomResponse(await _mediator.Send(new GetCategoryInPut(Id),cancellation));
        }

        [HttpGet("list-category")]
        public async Task<ActionResult<List<CategoryModelOutPut>>> ListCategory([FromQuery] ListCategoryInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request, cancellation));   
        }

        [HttpGet("list-category-products/{Id:Guid}")]
        public async Task<ActionResult<List<GetCategoryWithProductsOutPut>>> ListCategoryProdutcs(Guid Id, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(new GetCategoryWithProductsInPut(Id), cancellation));
        }

        [HttpPost]
        public async Task<ActionResult<CategoryModelOutPut>> Create(CreateCategoryInPut request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            return CustomResponse(await _mediator.Send(request, cancellationToken));
        }

        [HttpPut("{Id:Guid}")]
        public async Task<ActionResult<CategoryModelOutPut>> Update(Guid Id, UpdateCategoryInPut request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (Id != request.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(request);
            }
            return CustomResponse(await _mediator.Send(request, cancellationToken));
        }

        [HttpDelete("{Id:Guid}")]
        public async Task<ActionResult<CategoryModelOutPut>> Delete(Guid Id, CancellationToken cancellationToken)
        {
            return CustomResponse(await _mediator.Send(new DeleteCategoryInPut(Id), cancellationToken));        
        }

        [HttpGet("list-categories-cache")]

        public async Task<ActionResult<ListCategoriesCacheOutPut>> ListCategoryiesCache()
        {
            return CustomResponse(await _mediator.Send(new ListCategoriesCacheInPut(1,10,"","",SearchOrder.Desc)));
        }
    }
}
