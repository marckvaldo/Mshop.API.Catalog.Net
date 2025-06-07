using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Images.ListImage
{
    public class ListImage : BaseUseCase, IListImage
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        public ListImage(INotification notification, IImageRepository imageRepository, IProductRepository productRepositorys) : base(notification)
        {
            _imageRepository = imageRepository;
            _productRepository = productRepositorys;
        }
        public async Task<Result<ListImageOutPut>> Handle(ListImageInPut request, CancellationToken cancellation)
        {
            var product = await _productRepository.GetById(request.Id);
            
            if(NotifyErrorIfNull(product, "Não foi possivel localizar o produto na base de dados"))
                return Result<ListImageOutPut>.Error();

            var images = await _imageRepository.Filter(x=>x.ProductId == request.Id);

            var imagesOutPut = new ListImageOutPut
                    (
                        request.Id, 
                        images.Select(x => new ImageModelOutPut(x.FileName)).ToList()
                    );

            return Result<ListImageOutPut>.Success(imagesOutPut);
        }
    }
}
