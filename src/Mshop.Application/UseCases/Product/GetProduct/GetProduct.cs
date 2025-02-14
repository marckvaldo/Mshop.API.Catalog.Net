using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Product.GetProduct
{
    public class GetProduct : BaseUseCase, IGetProduct
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;

        public GetProduct(
            IProductRepository productRepository, 
            IImageRepository imageRepository, 
            INotification notification) : base(notification)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
        }

        public async Task<Result<GetProductOutPut>> Handle(GetProductInPut request, CancellationToken cancellation)
        {
            var product = await _productRepository.GetProductWithCategory(request.Id);
           
            if (NotifyErrorIfNull(product, "Não foi possivel localizar a produto da base de dados!"))
                return Result<GetProductOutPut>.Error();

            var images = await _imageRepository.Filter(x => x.ProductId == product.Id);

            var imagesOutPut = new GetProductOutPut(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.Thumb?.Path,
                product.Stock,
                product.IsActive,
                product.CategoryId,
                (new CategoryModelOutPut(product.CategoryId, product.Category.Name, product.Category.IsActive)),
                images.Select(x => x?.FileName).ToList(),
                product.IsSale) ;

            return Result<GetProductOutPut>.Success(imagesOutPut);
        }
    }
}
