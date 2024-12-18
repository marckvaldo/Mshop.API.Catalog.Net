using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductTestFixture : BaseFixture
    {
        private readonly Guid _categoryId;
        private readonly Guid _id;
        public List<string> fakeContantsNames;
        public ListProductTestFixture() : base()
        {
            _categoryId = Guid.NewGuid();
            _id = Guid.NewGuid();

            fakeContantsNames = new()
            {
                "ASP",
                "C#",
                "DARCK",
                "PHP"
            };
        }

        protected DomainEntity.Product Faker()
        {
            var product = (new DomainEntity.Product
            (
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription(),
                Convert.ToDecimal(faker.Commerce.Price()),
                _categoryId,
                _id,
                faker.Random.UInt(),
                true
            ));
            //product.Id = _id;
            product.AddCategory(new DomainEntity.Category(faker.Commerce.Categories(1)[0]));
            return product;
        }

        protected List<DomainEntity.Product> GetListProduts(int length = 10)
        {
            var products = new List<DomainEntity.Product>();
            for(int i = 0; i < length; i++)
                products.Add(Faker());
            return products;
        }

        protected IReadOnlyList<DomainEntity.Product> GetListProdutsConstant()
        {
            var products = GetListProduts(4);

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
