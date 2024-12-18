using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.UseCases.Category.GetCategory
{
    public class GetCategory : Core.Base.BaseUseCase, IGetCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategory(INotification notification, ICategoryRepository categoryRepository) : base(notification)
            => _categoryRepository = categoryRepository;

        public async Task<Result<CategoryModelOutPut>> Handle(GetCategoryInPut request, CancellationToken cancellationToken)
        {
            var category = await  _categoryRepository.GetById(request.Id);
            
            if (NotifyErrorIfNull(category, "não foi possivel localizar a categoria na base de dados!"))
                return Result<CategoryModelOutPut>.Error();

            if(!category!.IsValid(Notifications))
                return Result<CategoryModelOutPut>.Error();

           
            return Result<CategoryModelOutPut>.Success(CategoryModelOutPut.FromCategory(category));
        }

    }
}
