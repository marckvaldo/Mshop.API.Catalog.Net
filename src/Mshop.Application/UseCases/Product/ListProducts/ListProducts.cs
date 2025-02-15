using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Core.Paginated;
using Mshop.Infra.Data.Interface;


namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProducts : Core.Base.BaseUseCase, IListProducts
    {
        private readonly IProductRepository _productRepostory;

        public ListProducts(IProductRepository productRepostory, INotification notification) : base(notification)
            => _productRepostory = productRepostory;   
        public async Task<Result<ListProductsOutPut>> Handle(ListProductInPut request, CancellationToken cancellation)
        {
            var paginatedInPut = new PaginatedInPut(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                request.Dir
                );

            var paginateOutPut = await _productRepostory.FilterPaginatedQuery(paginatedInPut, request.CategoryId, request.OnlyPromotion, CancellationToken.None);


            var listProdutosOutPut = new ListProductsOutPut(
                paginateOutPut.CurrentPage,
                paginateOutPut.PerPage,
                paginateOutPut.Total,
                paginateOutPut.Itens.Select(x => new ProductModelOutPut
                (
                    x.Id,
                    x.Description,
                    x.Name,
                    x.Price,
                    x.Thumb?.Path,
                    x.Stock,
                    x.IsActive,
                    x.CategoryId,
                    (new CategoryModelOutPut(x.Category.Id,x.Category.Name,x.Category.IsActive))
                )).ToList());

            return Result<ListProductsOutPut>.Success(listProdutosOutPut);
        }
    }
}
