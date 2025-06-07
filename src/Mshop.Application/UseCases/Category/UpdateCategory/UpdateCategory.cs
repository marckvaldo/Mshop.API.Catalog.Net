using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Category.CreateCategory;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Domain.Event.Category;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Category.UpdateCategory
{
    public class UpdateCategory : Core.Base.BaseUseCase, IUpdateCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCategory(
            ICategoryRepository categoryRepository, 
            INotification notification,
            IUnitOfWork unitOfWork
            ) : base(notification)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
            

        public async Task<Result<CategoryModelOutPut>> Handle(UpdateCategoryInPut request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetById(request.Id);
            if (NotifyErrorIfNull(category, "não foi possivel localizar a categoria da base de dados!"))
                return Result<CategoryModelOutPut>.Error();

            category!.Update(request.Name);

            if (request.IsActive)
                category.Active();
            else
                category.Deactive();

            if(!category.IsValid(Notifications))
                return Result<CategoryModelOutPut>.Error();

            category.RegisterEvent(new CategoryUpdatedEvent(category.Id));
            await _categoryRepository.Update(category,cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);


            return Result<CategoryModelOutPut>.Success(CategoryModelOutPut.FromCategory(category));
        }
    }
}
