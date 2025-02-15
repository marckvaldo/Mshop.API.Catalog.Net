using Mshop.Catalog.E2ETests.Base;
using Mshop.Core.Enum.Paginated;
using Entity = Mshop.Domain.Entity;


namespace MShop.Catalog.E2ETest.GraphQL.Category
{
    public class CategoryFixtureTest : BaseWebApplication
    {
        public CategoryFixtureTest() : base(TypeProjetct.GraphQL)
        {
           
        }

        public IList<Entity.Category> GetListCategories(IEnumerable<string> categoryName)
        {
            return categoryName.Select(x => new Entity.Category(x,true )).ToList();
        }


        public IList<Entity.Category> CloneListCategorySort(IList<Entity.Category> categories, string orderBy, SearchOrder order)
        {
            var listClone = new List<Entity.Category>(categories);
            switch (orderBy.ToLower(), order)
            {
                case ("name", SearchOrder.Desc):
                    listClone = listClone.OrderByDescending(x => x.Name).ToList();
                    break;
                case ("name", SearchOrder.Asc):
                    listClone = listClone.OrderBy(x => x.Name).ToList();
                    break;
                case ("id", SearchOrder.Asc):
                    listClone = listClone.OrderBy(x => x.Id).ToList();
                    break;
                case ("id", SearchOrder.Desc):
                    listClone = listClone.OrderByDescending(x => x.Id).ToList();
                    break;
                default:
                    listClone = listClone.OrderBy(x => x.Name).ToList();
                    break;
            }

            return listClone;
        }
    }
}
