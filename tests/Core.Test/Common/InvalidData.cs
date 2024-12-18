namespace Mshop.Core.Test.Common
{
    public class InvalidData
    {
        public static string GetNameProductGreaterThan255CharactersInvalid()
        {

            string description = BaseFixture.fakerStatic.Commerce.ProductName();
            while (description.Length < 256)
            {
                description += BaseFixture.fakerStatic.Commerce.ProductName();
            }

            return description;
        }
        public static string GetNameProductLessThan3CharactersInvalid()
        {
            string product = BaseFixture.fakerStatic.Commerce.ProductName();
            product = product[..2];
            return product;
        }

        public static string GetDescriptionProductGreaterThan1000CharactersInvalid()
        {
            string description = BaseFixture.fakerStatic.Commerce.ProductDescription();
            while (description.Length < 1001)
            {
                description += BaseFixture.fakerStatic.Commerce.ProductDescription();
            }

            return description;
        }

        public static string GetDescriptionProductLessThan10CharactersInvalid()
        {
            string category = BaseFixture.fakerStatic.Commerce.ProductDescription();
            category = category[..9];
            return category;
        }



        //Category
        public static string GetNameCategoryGreaterThan30CharactersInvalid()
        {
            string category = BaseFixture.fakerStatic.Commerce.Categories(1)[0];
            while (category.Length < 32)
            {
                category += BaseFixture.fakerStatic.Commerce.Categories(1)[0];
            }
            return category;
        }

        public static string GetNameCategoryLessThan3CharactersInvalid()
        {
            string category = BaseFixture.fakerStatic.Commerce.Categories(1)[0];
            category = category[..2];
            return category;
        }

    }
}
