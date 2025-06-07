using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Cache.Category.GetCategory;
using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Cache.Products.GetProductCache;
using Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache;
using Mshop.Application.UseCases.Cache.Products.ListProductCache;
using Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Product.Common;


namespace Mshop.API.Catalog.Controllers.v1
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CacheController : MainController
    {

        private readonly IMediator _mediator;
        public CacheController(
            IMediator mediator,
            Core.Message.INotification notification) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(ListCategoriesCacheOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ListCategoriesCacheOutPut>> ListCategoryiesCache([FromQuery] ListCategoriesCacheInPut request, CancellationToken cancellation)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);

            var result = await _mediator.Send(request, cancellation);
            return CustomResponse(result);
        }


        [HttpGet("categories/{Id:Guid}")]
        [ProducesResponseType(typeof(ListCategoriesCacheOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryModelOutPut>> CategoryCache(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryCacheInPut(id));
            return CustomResponse(result);
        }


        [HttpGet("products")]
        [ProducesResponseType(typeof(ListCategoriesCacheOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ListProductCacheOutPut>> ListProductsCache([FromQuery] ListProductCacheInPut request, CancellationToken cancellation)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);
            var result = await _mediator.Send(request, cancellation);
            return CustomResponse(result);
        }

        [HttpGet("products/{Id:Guid}")]
        [ProducesResponseType(typeof(ListCategoriesCacheOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetProductCacheOutPut>> ProductCache(Guid id)
        {
            var result = await _mediator.Send(new GetProductCacheInPut(id));
            return CustomResponse(result);
        }


    }
}
