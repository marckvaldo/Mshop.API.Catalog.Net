using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.Application.UseCases.Product.DeleteProduct;
using Mshop.Application.UseCases.Product.GetProduct;
using Mshop.Application.UseCases.Product.ListProducts;
using Mshop.Application.UseCases.Product.UpdateProduct;
using Mshop.Application.UseCases.Product.UpdateStockProduct;
using Mshop.Application.UseCases.Product.UpdateThumb;

namespace Mshop.API.Catalog.Controllers.v1
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : MainController
    {
        private readonly IMediator _mediator;
        public ProductsController(
            Core.Message.INotification notification,
            IMediator mediator
            ) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductModelOutPut>>> Product(Guid id, CancellationToken cancellation)
        {
            var result = await _mediator.Send(new GetProductInPut(id), cancellation);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProductModelOutPut>>> ListProdutcs([FromQuery] ListProductInPut request, CancellationToken cancellation)
        {
            if(!ModelState.IsValid) CustomResponse(ModelState);
            var result = await _mediator.Send(request, cancellation);
            return CustomResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductModelOutPut>> Create([FromBody] CreateProductInPut product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            return CustomResponse(await _mediator.Send(product, cancellationToken), 201);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModelOutPut>> Update(Guid id, UpdateProductInPut product, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }
            var result = await _mediator.Send(product, cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);

        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModelOutPut>> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteProductInPut(id), cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpPut("update-stock/{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModelOutPut>> UpdateStock(Guid id, [FromBody] UpdateStockProductInPut product, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }

            var result = await _mediator.Send(product, cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);

        }

        [HttpPut("update-thump/{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModelOutPut>> UpdateThumb(Guid id, [FromBody] UpdateThumbInPut product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);

            if (id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }
            var result = await _mediator.Send(product, cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }
    }
}

