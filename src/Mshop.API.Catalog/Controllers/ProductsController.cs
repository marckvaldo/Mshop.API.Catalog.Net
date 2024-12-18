using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Application.UseCases.Product.CreateProducts;
using Mshop.Application.UseCases.Product.DeleteProduct;
using Mshop.Application.UseCases.Product.GetProduct;
using Mshop.Application.UseCases.Product.ListProducts;
using Mshop.Application.UseCases.Product.UpdateProduct;
using Mshop.Application.UseCases.Product.UpdateStockProduct;
using Mshop.Application.UseCases.Product.UpdateThumb;

namespace Mshop.API.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : MainController
    {
        /*private readonly IGetProduct _getProduct;
        private readonly ICreateProduct _createProduct;
        private readonly IUpdateProduct _updateProduct;
        private readonly IDeleteProduct _deleteProduct;
        private readonly IUpdateStockProduct _updateStoqueProduct;
        private readonly IListProducts _listProducts;
        private readonly IProductsPromotions _productPromotions;
        private readonly IUpdateThumb _updateThumb;*/

        private readonly IMediator _mediator;

        public ProductsController(
            /*IGetProduct getProduct, 
            ICreateProduct createProduct, 
            IUpdateProduct updateProduct, 
            IDeleteProduct deleteProduct,
            IUpdateStockProduct updateStoqueProduct,
            IListProducts listProducts,
            INotification notification,
            IProductsPromotions productPromotions,
            IUpdateThumb updateThumb*/
            Core.Message.INotification notification,
            IMediator mediator
            ) : base(notification)
        {
            /*_getProduct = getProduct;
            _createProduct = createProduct;
            _updateProduct = updateProduct;
            _deleteProduct = deleteProduct;
            _updateStoqueProduct = updateStoqueProduct;
            _listProducts = listProducts;
            _productPromotions = productPromotions;
            _updateThumb = updateThumb;*/

            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IEnumerable<ProductModelOutPut>>> Product(Guid id, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(new GetProductInPut(id), cancellation));        
        }

        [HttpGet("list-products")]
        public async Task<ActionResult<List<ProductModelOutPut>>> ListProdutcs([FromQuery] ListProductInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request, cancellation));
        }

        [HttpGet("list-products-promotions")]
        public async Task<ActionResult<List<ProductModelOutPut>>> ListProdutcsPromotions()
        {
            //return CustomResponse(await _productPromotions.Handler());
            return CustomResponse();
        }

        [HttpPost]
        public async Task<ActionResult<ProductModelOutPut>> Create([FromBody] CreateProductInPut product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            return CustomResponse(await _mediator.Send(product, cancellationToken));      
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductModelOutPut>> Update(Guid id, UpdateProductInPut product, CancellationToken cancellationToken)
        {
        
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }

            return CustomResponse(await _mediator.Send(product, cancellationToken));
        
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProductModelOutPut>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return CustomResponse(await _mediator.Send(new DeleteProductInPut(id), cancellationToken));
        }

        [HttpPost("update-stock/{id:guid}")]
        public async Task<ActionResult<ProductModelOutPut>> UpdateStock(Guid id, [FromBody] UpdateStockProductInPut product, CancellationToken cancellationToken)
        {
       
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }

            return CustomResponse(await _mediator.Send(product, cancellationToken));
        
        }

        [HttpPut("update-thump/{id:guid}")]
        public async Task<ActionResult<ProductModelOutPut>> UpdateThumb(Guid id, [FromBody] UpdateThumbInPut product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);

            if(id != product.Id)
            {
                Notify("O id informado não é o mesmo passado como parametro");
                return CustomResponse(product);
            }

            return CustomResponse(await _mediator.Send(product, cancellationToken));
        }
    }
}

