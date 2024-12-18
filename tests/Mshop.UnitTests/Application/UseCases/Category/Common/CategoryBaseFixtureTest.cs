using Mshop.UnitTests.Common;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Category.Common;

public class CategoryBaseFixtureTest : BaseFixture
{
   

    public DomainEntity.Category FakerCategory()
    {
        return new DomainEntity.Category(faker.Commerce.Categories(1)[0]);
    }

    public List<DomainEntity.Category> FakerCategories(int quantity)
    {
        List<DomainEntity.Category> listCategory = new List<DomainEntity.Category>();
        for (int i = 1; i <= quantity; i++)
            listCategory.Add(FakerCategory());

        return listCategory;
    }


    public static IEnumerable<object[]> ListNamesCategoryInvalid()
    {
        yield return new object[] { InvalidData.GetNameCategoryGreaterThan30CharactersInvalid() };
        yield return new object[] { InvalidData.GetNameCategoryLessThan3CharactersInvalid() };
        yield return new object[] { "" };
        yield return new object[] { null };
    }

    public DomainEntity.Product FakerProduct(DomainEntity.Category category)
    {
        var product = new DomainEntity.Product
            (
                 faker.Commerce.ProductName(),
                 faker.Commerce.ProductDescription(),
                Convert.ToDecimal(faker.Commerce.Price()),
                category.Id,
                faker.Random.UInt(),
                true
            );

        return product;
    }

    /*public DomainEntity.Category FakerCategory()
    {
        var category = new DomainEntity.Category
            (
                 faker.Commerce.Categories(1)[0],
                 true
            );

        return category;
    }*/

    public List<DomainEntity.Product> FakerProducts(int quantity, DomainEntity.Category category)
    {
        List<DomainEntity.Product> listProduct = new List<DomainEntity.Product>();
        for (int i = 1; i <= quantity; i++)
            listProduct.Add(FakerProduct(category));

        return listProduct;
    }
}
