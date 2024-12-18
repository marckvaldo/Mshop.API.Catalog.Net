using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.Base;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategory : BaseUseCase, IDeleteCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCategory(
            ICategoryRepository categoryRepository, 
            IProductRepository productRepository,
            INotification notification, 
            IUnitOfWork unitOfWork) : base(notification)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryModelOutPut>> Handle(DeleteCategoryInPut request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetById(request.Id);
           
            if (NotifyErrorIfNull(category, "não foi possivel localizar a categoria da base de dados!"))
                return Result<CategoryModelOutPut>.Error();
            
            var products = await _productRepository.GetProductsByCategoryId(request.Id);
            if (products?.Count > 0)
            {
                Notify("Não é possivel excluir um categoria quando a mesma ja está relacionada com produtos");
                return Result<CategoryModelOutPut>.Error();
            }
            
           
            await _categoryRepository.DeleteById(category!,cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<CategoryModelOutPut>.Success(CategoryModelOutPut.FromCategory(category));
        }


    }
}
