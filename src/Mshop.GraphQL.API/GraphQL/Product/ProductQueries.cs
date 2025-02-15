using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Cache.Products.GetProductCache;
using Mshop.Application.UseCases.Cache.Products.ListProductCache;
using Mshop.Application.UseCases.Images.ListImage;
using Mshop.Core.Enum.Paginated;
using Notification = Mshop.Core.Message;

namespace Mshop.API.GraphQL.GraphQL.Product
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ProductQueries : BaseGraphQL
    {
        public async Task<ProductPayload> GetProductById(
            [Service] IMediator mediator,
            [Service] Notification.INotification notification,
            Guid id,
            CancellationToken cancellationToken)
        {
            var request = new GetProductCacheInPut(id);
            var outPut = await mediator.Send(request, cancellationToken);

            var requestImage = new ListImageInPut(id);
            var outPutImage = await mediator.Send(requestImage, cancellationToken);

            RequestIsValid(notification);

            var result = outPut.Data;
            var resultImage = outPutImage.Data;

            var produto = new ProductPayload(result.Id,
                    result.Description,
                    result.Name,
                    result.Price,
                    result.Thumb,
                    result.Stock,
                    result.IsActive,
                    result.CategoryId,
                    new Category.CategoryPayload(result.Category.Id,result.Name,result.IsActive),
                    result.IsPromotion);

            resultImage.Images.ForEach(x => produto.AddImages(x.Image));
            //produto.AddImages();

            return produto;
        }

        public async Task<ProductSearchOutPut> ListProductCacheOutPutAsync(
            [Service] IMediator mediator,
            [Service] Notification.INotification notification,
            int perPage = 10,
            int page = 1,
            string search = "",
            string sort = "",
            SearchOrder order = SearchOrder.Asc,
            bool onlyPromotion = false,
            Guid categoryId = default,
            CancellationToken cancellationToken = default)
        {
    
        var request = new ListProductCacheInPut(page, perPage, search, sort, order, onlyPromotion, categoryId);
        var outPut = await mediator.Send(request, cancellationToken);

        RequestIsValid(notification);

        var result = outPut.Data;
        return new ProductSearchOutPut(result.Page, result.PerPage, result.Total,
            result.Itens.Select(x => new ProductPayload(
                x.Id,
                x.Description,
                x.Name,
                x.Price,
                x.Thumb,
                x.Stock,
                x.IsActive,
                x.CategoryId,
                new Category.CategoryPayload(x.CategoryId, x.Category.Name, x.Category.IsActive),
                x.IsPromotion)).ToList());
        }
    }
}
