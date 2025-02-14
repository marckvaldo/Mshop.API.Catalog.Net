using Mshop.Application.UseCases.Category.Common;

namespace Mshop.API.GraphQL.GraphQL.Category
{
    public class CategoryPayload : CategoryModelOutPut
    {
        public CategoryPayload(Guid id, string name, bool isActive) : base(id, name, isActive)
        {

        }
    }
}
