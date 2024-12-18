using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Domain.Contract.Services;
using Mshop.Core.Base;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Product.UpdateThumb
{
    public class UpdateThumb : BaseUseCase, IUpdateThumb
    {
        private readonly IProductRepository _productRepository;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateThumb(
            IProductRepository productRepository, 
            IStorageService storageService,
            INotification notification,
            IUnitOfWork unitOfWork) : base(notification)
        {
            _productRepository = productRepository;
            _storageService = storageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductModelOutPut>> Handle(UpdateThumbInPut request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetById(request.Id);
            
            if(NotifyErrorIfNull(product, "Não foi possivel localizar o produto!"))
                return Result<ProductModelOutPut>.Error();

            if(!product!.IsValid(Notifications))
                return Result<ProductModelOutPut>.Error();
           

            await UploadImage(request, product);


            if(NotifyErrorIfNull(product.Events.Count == 0 ? null : product.Events, $" Não foi possivel registrar o event ProductUpdatedEvent"))
                return Result<ProductModelOutPut>.Error();

            await _productRepository.Update(product, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var productOutPut = new ProductModelOutPut(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.Thumb?.Path,
                product.Stock,
                product.IsActive,
                product.CategoryId,
                null,
                product.IsSale
                );

            return Result<ProductModelOutPut>.Success(productOutPut);
        }

        private async Task UploadImage(UpdateThumbInPut request, Domain.Entity.Product product)
        {
            if (string.IsNullOrEmpty(request.Thumb?.FileStremBase64.Trim()))
                return;

            var thumb = Helpers.Base64ToStream(request.Thumb.FileStremBase64);
            var urlThumb = await _storageService.Upload($"{product.Id}-thumb.{thumb.Extension}", thumb.FileStrem);

            if (!string.IsNullOrEmpty(request.Thumb.FileStremBase64.Trim()) && !string.IsNullOrEmpty(product.Thumb?.Path.Trim()))
                await _storageService.Delete(product.Thumb.Path);

            product.UpdateThumb(urlThumb);

        }
    }
}
