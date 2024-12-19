using Mshop.Core.Test.UseCase;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductTestFixture : UseCaseBaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;
        public List<string> fakeContantsNames;

        public ListProductTestFixture() : base()
        {
            fakeContantsNames = new()
            {
                "ASP",
                "C#",
                "DARCK",
                "PHP"
            };
        }

        protected IReadOnlyList<DomainEntity.Product> GetListProdutsConstant()
        {
            var category = FakerCategory();
            var products = FakerProducts(10, category);
            //return products;
            //var products = GetListProduts(4);

            int i = 0;
            foreach (var item in products)
            {
                item.Update(item.Description, fakeContantsNames[i], item.Price, item.CategoryId);
                i++;
            }
            

            return products; 
        }
    }
}
