using Mshop.Application.UseCases.Product.Common;
using Mshop.Core.Base;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public class UpdateStockProducts : BaseUseCase, IUpdateStockProduct
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStockProducts(
            IProductRepository productRepository, 
            INotification notification,
            IUnitOfWork unitOfWork,
            ICategoryRepository categoryRepository) : base(notification)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<ProductModelOutPut>> Handle(UpdateStockProductInPut request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetById(request.Id);
              
            if (NotifyErrorIfNull(product, "Não foi possivel localizar a produto da base de dados!"))
                return Result<ProductModelOutPut>.Error();

            var category = await _categoryRepository.GetById(product.CategoryId);

            product!.UpdateQuantityStock(request.Stock);

            if(!product.IsValid(Notifications))
                return Result<ProductModelOutPut>.Error();

           
            await _productRepository.Update(product, cancellationToken);

            //if(NotifyErrorIfNull(product.Events.Count == 0 ? null : product.Events, $" Não foi possivel registrar o event ProductUpdatedEvent"))
                //return Result<ProductModelOutPut>.Error();

            await _unitOfWork.CommitAsync(cancellationToken);

            var produtoOutPut =  new ProductModelOutPut(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.Thumb?.Path,
                product.Stock,
                product.IsActive,
                product.CategoryId);

            return Result<ProductModelOutPut>.Success(produtoOutPut);
        }
    }
}
