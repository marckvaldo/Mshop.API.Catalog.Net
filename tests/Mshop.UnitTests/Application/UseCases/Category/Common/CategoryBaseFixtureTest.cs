using Mshop.Core.Test.Common;
using Mshop.Core.Test.UseCase;

namespace Mshop.Application.UseCases.Category.Common;

public class CategoryBaseFixtureTest : UseCaseBaseFixture
{
    public CategoryBaseFixtureTest() : base()
    {

    }

    public static IEnumerable<object[]> ListNamesCategoryInvalid()
    {
        yield return new object[] { InvalidData.GetNameCategoryGreaterThan30CharactersInvalid() };
        yield return new object[] { InvalidData.GetNameCategoryLessThan3CharactersInvalid() };
        yield return new object[] { "" };
        yield return new object[] { null };
    }
}
