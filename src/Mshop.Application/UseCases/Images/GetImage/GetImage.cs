using Mshop.Application.UseCases.Images.Common;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using Mshop.Domain.Contract.Services;

namespace Mshop.Application.UseCases.Images.GetImage
{
    public class GetImage : BaseUseCase, IGetImage
    {
        private readonly IImageRepository _imageRepository;
        private readonly IStorageService _storageService;
        public GetImage(INotification notification, 
            IImageRepository imageRepository, 
            IStorageService storageService) : base(notification)
        {
            _storageService = storageService;   
            _imageRepository = imageRepository; 
        }

        public async Task<Result<ImageOutPut>> Handle(GetImageInPut request, CancellationToken cancellation)
        {
            var image = await _imageRepository.GetById(request.Id);
           
            if (NotifyErrorIfNull(image, ""))
                return Result<ImageOutPut>.Error();

            var imageOutPut =  new ImageOutPut(image!.ProductId, new ImageModelOutPut(image.FileName));
            return Result<ImageOutPut>.Success(imageOutPut);

        }
    }
}
