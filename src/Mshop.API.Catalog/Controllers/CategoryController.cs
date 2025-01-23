using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Application.UseCases.Category.DeleteCategory;
using Mshop.Application.UseCases.Category.GetCategory;
using Mshop.Application.UseCases.Category.GetCatetoryWithProducts;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Application.UseCases.Category.UpdateCategory;


namespace Mshop.API.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : MainController
    {
        private readonly IMediator _mediator;
        public CategoryController(
            IMediator mediator,
            Core.Message.INotification notification) : base(notification)
        {
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

    }
}
