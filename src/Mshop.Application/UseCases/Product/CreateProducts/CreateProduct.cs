using Mshop.Application.Common;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using Mshop.Domain.Contract.Services;

namespace Mshop.Application.UseCases.Product.CreateProducts
{
    public class CreateProduct : Core.Base.BaseUseCase, ICreateProduct
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;        

        public CreateProduct(IProductRepository productRepository, 
            INotification notification,
            ICategoryRepository categoryRepository,
            IStorageService storageService,
            IUnitOfWork unitOfWork) : base(notification)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _storageService = storageService;  
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductModelOutPut>> Handle(CreateProductInPut request, CancellationToken cancellationToken)
        {
            var product = new Domain.Entity.Product(
                    request.Description,
                    request.Name,
                    request.Price,
                    request.CategoryId,
                    request.Stock,
                    request.IsActive
                );

            if (!product.IsValid(Notifications))
                return Result<ProductModelOutPut>.Error();

            var category = await _categoryRepository.GetById(product.CategoryId);
            
            if(NotifyErrorIfNull(category, $"Categoria {product.CategoryId} não encontrada"))
                return Result<ProductModelOutPut>.Error();

            try
            {
                await UploadImage(request, product);

               
                //if(NotifyErrorIfNull(product.Events.Count == 0 ? null : product.Events, $" Não foi possivel registrar o event ProductCreatedEvent"))
                    //return Result<ProductModelOutPut>.Error();

                await _productRepository.Create(product,cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);


                var produtoOutPut =  new ProductModelOutPut(
                        product.Id,
                        product.Description,
                        product.Name,
                        product.Price,
                        product.Thumb?.Path,
                        product.Stock,
                        product.IsActive,
                        product.CategoryId
                    );

                return Result<ProductModelOutPut>.Success(produtoOutPut);
            }
            catch(Exception)
            {
                if(product?.Thumb?.Path is not null) 
                    await _storageService.Delete(product.Thumb.Path);
                throw;
            }
        }

        private async Task UploadImage(CreateProductInPut request, Domain.Entity.Product product)
        {
            if (string.IsNullOrEmpty(request.Thumb?.FileStremBase64.Trim()))
                return;
           

            var thumbFile = Helpers.Base64ToStream(request.Thumb.FileStremBase64);
            var urlThumb = await _storageService.Upload($"{product.Id}-thumb.{thumbFile.Extension}", thumbFile.FileStrem);
            product.UpdateThumb(urlThumb);
            
        }
    }
}
