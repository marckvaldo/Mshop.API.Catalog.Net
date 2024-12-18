using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Message = Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Category.CreateCategory
{
    public class CreateCategory : Core.Base.BaseUseCase, ICreateCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategory(
            Message.INotification notification,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork) : base(notification)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryModelOutPut>> Handle(CreateCategoryInPut request, CancellationToken cancellationToken)
        {
            var category = new Domain.Entity.Category(request.Name, request.IsActive);

            if(!category.IsValid(Notifications))
                return Result<CategoryModelOutPut>.Error(Notifications);


            var isThereCatetory = await _categoryRepository.GetByName(request.Name);
            if (isThereCatetory is not null)
            {
                Notify("Categoria ja existe na base de dados");
                return Result<CategoryModelOutPut>.Error(Notifications);
            }
                
       

            await _categoryRepository.Create(category,cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
           
            return Result<CategoryModelOutPut>.Success(CategoryModelOutPut.FromCategory(category));

        }

    }
}
