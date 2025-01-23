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


namespace Mshop.API.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : MainController
    {

        private readonly IMediator _mediator;
        public CacheController(
            IMediator mediator,
            Core.Message.INotification notification) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("list-categories-cache")]
        public async Task<ActionResult<ListCategoriesCacheOutPut>> ListCategoryiesCache([FromQuery] ListCategoriesCacheInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request,cancellation));
        }


        [HttpGet("category-cache/{Id:Guid}")]
        public async Task<ActionResult<CategoryModelOutPut>> CategoryCache(Guid id)
        {
            return CustomResponse(await _mediator.Send(new GetCategoryCacheInPut(id)));
        }



        [HttpGet("list-products-cache")]
        public async Task<ActionResult<ListProductCacheOutPut>> ListProductsCache([FromQuery] ListProductCacheInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request, cancellation));
        }

        [HttpGet("list-products-by-category-cache")]
        public async Task<ActionResult<ListProductByCategoryCacheOutPut>> ListProductsByCategoryCache([FromQuery] ListProductByCategoryCacheInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request, cancellation));
        }


        [HttpGet("list-products-promotion-cache")]
        public async Task<ActionResult<ListProductPromotionCacheOutPut>> ListProductsPromotionCache([FromQuery] ListProductPromotionCacheInPut request, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(request, cancellation));
        }


        [HttpGet("product-cache/{Id:Guid}")]
        public async Task<ActionResult<GetProductCacheOutPut>> ProductCache(Guid id)
        {
            return CustomResponse(await _mediator.Send(new GetProductCacheInPut(id)));
        }


    }
}
