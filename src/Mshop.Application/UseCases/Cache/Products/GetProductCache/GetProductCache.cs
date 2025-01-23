using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Cache.Products.GetProductCache
{
    public class GetProductCache : BaseUseCase, IGetProductCache
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCacheRepository _productCacheRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IImagesCacheRepository _imageCacheRepository;


        private readonly IBuildCacheProduct _buildCache;
        private readonly IBuildCacheImage _buildCacheImage;

        public GetProductCache(
            IProductRepository productRepository,
            IProductCacheRepository productCacheRepository,
            IImageRepository imageRepository,
            IImagesCacheRepository imageCacheRepository,
            IBuildCacheProduct buildCache,
            IBuildCacheImage buildCacheImage,
            INotification notification) : base(notification)
        {
            _productRepository = productRepository;
            _productCacheRepository = productCacheRepository;
            _imageRepository = imageRepository; 
            _imageCacheRepository = imageCacheRepository;
            _buildCache = buildCache;
            _buildCacheImage = buildCacheImage;
        }

        public async Task<Result<GetProductCacheOutPut>> Handle(GetProductCacheInPut request, CancellationToken cancellation)
        {
            var product = await _productCacheRepository.GetById(request.Id);

            if(product is null)
            {
                _buildCache.Handle();
                product = await _productRepository.GetProductWithCategory(request.Id);
            }

            if (NotifyErrorIfNull(product, "Não foi possivel localizar a produto da base de dados!"))
                return Result<GetProductCacheOutPut>.Error();

            var images = await _imageCacheRepository.GetImageByProductId(request.Id);

            if(images is null)
            {
                _buildCacheImage.Handle();
                images = await _imageRepository.Filter(x=> x.ProductId == product.Id);
            }

            var imagesOutPut = new GetProductCacheOutPut(
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

            return Result<GetProductCacheOutPut>.Success(imagesOutPut);
        }
    }
}
