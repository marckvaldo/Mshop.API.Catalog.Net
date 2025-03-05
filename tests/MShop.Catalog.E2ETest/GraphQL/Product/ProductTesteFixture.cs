using Mshop.Catalog.E2ETests.Base;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity = Mshop.Domain.Entity;

namespace Mshop.Catalog.E2ETests.GraphQL.Product
{
    public class ProductTesteFixture : BaseWebApplication
    {
        public ProductTesteFixture():base(TypeProjetct.GraphQL) { }


        public IList<Entity.Product> CloneListProductSort(IList<Entity.Product> products, string orderBy, SearchOrder order)
        {
            var listClone = new List<Entity.Product>(products);
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

        public IList<Entity.Product> GetListProducts(List<string> productName)
        {
            var category = FakerCategory();
            var products = FakerProducts(productName.Count(), category);
            int i = 0;
            products.ForEach(x => x.Update(x.Description, productName[i++], x.Price, x.CategoryId));
            return products;
        }

        public async void ClearProductDataBase(IProductRepository repository, IUnitOfWork unitOfWork)
        {
            var products = await repository.GetProductAll();
            foreach(var item in products)
            {
                await repository.DeleteById(item, CancellationToken.None);
            }

            await unitOfWork.CommitAsync();
        }
    }
}
