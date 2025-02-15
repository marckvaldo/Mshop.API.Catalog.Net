using MediatR;
using Mshop.Application.UseCases.Cache.Category.GetCategory;
using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Category.GetCategory;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Core.Enum.Paginated;
using Notification = Mshop.Core.Message;

namespace Mshop.API.GraphQL.GraphQL.Category
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CategoryQueries : BaseGraphQL
    {
        public async Task<CategoryPayload> GetCategoryById(
            [Service] IMediator mediator,
            [Service] Notification.INotification notification,
            Guid id, 
            CancellationToken cancellationToken)
        {
            var request = new GetCategoryCacheInPut(id);
            var outPut = await mediator.Send(request, cancellationToken);

            RequestIsValid(notification);

            return new CategoryPayload(
                outPut.Data.Id,
                outPut.Data.Name,
                outPut.Data.IsActive);
        }
        
        public async Task<CategorySeachPayload> ListCategories(
            [Service] IMediator mediator,
            [Service] Notification.INotification notification,
            int perPage = 10,
            int page = 1,
            string search = "",
            string orderBy = "",
            SearchOrder order = SearchOrder.Asc,
            CancellationToken cancellationToken = default)
        {
            

            var request = new ListCategoriesCacheInPut(page, perPage, search, orderBy, order);
            var outPut = await mediator.Send(request, cancellationToken);
            
            RequestIsValid(notification);

            return new CategorySeachPayload(
                    outPut.Data.Page,
                    outPut.Data.PerPage,
                    outPut.Data.Total,
                    outPut.Data.Itens.Select(x => new CategoryPayload(
                        x.Id,
                        x.Name,
                        x.IsActive)).ToList()
                    );
        }

    }
}
