using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Application.UseCases.Category.DeleteCategory;
using Mshop.Application.UseCases.Category.GetCategory;
using Mshop.Application.UseCases.Category.GetCatetoryWithProducts;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Application.UseCases.Category.UpdateCategory;


namespace Mshop.API.Catalog.Controllers.v1
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CategoriesController : MainController
    {
        private readonly IMediator _mediator;
        public CategoriesController(
            IMediator mediator,
            Core.Message.INotification notification) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("{Id:Guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutPut>> Category(Guid Id, CancellationToken cancellation)
        {
            var result = await _mediator.Send(new GetCategoryInPut(Id), cancellation);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<CategoryModelOutPut>>> ListCategory([FromQuery] ListCategoryInPut request, CancellationToken cancellation)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);
            var result = await _mediator.Send(request, cancellation);
            return CustomResponse(result);
        }


        [HttpGet("list-category-and-productId/{Id:Guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<GetCategoryWithProductsOutPut>>> ListCategoryProdutcs(Guid Id, CancellationToken cancellation)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);
            var result = await _mediator.Send(new GetCategoryWithProductsInPut(Id), cancellation);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryModelOutPut>> Create(CreateCategoryInPut request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var result = await _mediator.Send(request, cancellationToken);
            return CustomResponse(result, 201);
        }


        [HttpPut("{Id:Guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutPut>> Update(Guid Id, UpdateCategoryInPut request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (Id != request.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(request);
            }
            
            var result = await _mediator.Send(request, cancellationToken);
            if (result.Data is null) return CustomResponse(404);

            return CustomResponse(result);
        }


        [HttpDelete("{Id:Guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutPut>> Delete(Guid Id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCategoryInPut(Id), cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

    }
}
