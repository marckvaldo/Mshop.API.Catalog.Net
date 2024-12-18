using Mshop.Core.Test.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Core.Test.Domain
{
    public class DomainEntityFixture : BaseFixture
    {
        public DomainEntityFixture() : base() { }


        /*Category*/
        public static IEnumerable<object[]> ListNamesCategoryInvalid()
        {
            yield return new object[] { InvalidData.GetNameCategoryGreaterThan30CharactersInvalid() };
            yield return new object[] { InvalidData.GetNameCategoryLessThan3CharactersInvalid() };
            yield return new object[] { "" };
            yield return new object[] { null };
        }

        public DomainEntity.Category FakerCategory()
        {
            return new DomainEntity.Category(GetNameCategoryValid());
        }

        protected DomainEntity.Category GetCategoryValid(string name, bool isActive = true)
        {
            return new(name, isActive);
        }


        /*Image*/

        public DomainEntity.Image FakerImage(Guid productId)
        {
            return new DomainEntity.Image(faker.Image.LoremFlickrUrl(), productId);
        }

        public DomainEntity.Image FakerImage(Guid productId, string FileName)
        {
            return new DomainEntity.Image(FileName, productId);
        }


        /*Product*/

        public static IEnumerable<object[]> ListNameProductInvalid()
        {
            yield return new object[] { InvalidData.GetNameProductGreaterThan255CharactersInvalid() };
            yield return new object[] { InvalidData.GetNameProductLessThan3CharactersInvalid() };
            yield return new object[] { "" };
            yield return new object[] { " " };
            yield return new object[] { null };
        }

        public static IEnumerable<object[]> ListDescriptionProductInvalid()
        {
            yield return new object[] { InvalidData.GetDescriptionProductGreaterThan1000CharactersInvalid() };
            yield return new object[] { InvalidData.GetDescriptionProductLessThan10CharactersInvalid() };
            yield return new object[] { "" };
            yield return new object[] { " " };
            yield return new object[] { null };
        }

        public DomainEntity.Product FakerProduct(DomainEntity.Category category)
        {
            var product = (new DomainEntity.Product
            (
                faker.Commerce.ProductDescription(),
                faker.Commerce.ProductName(),
                Convert.ToDecimal(faker.Commerce.Price()),
                category.Id,
                faker.Random.UInt(),
                true
            ));

            product.UpdateThumb(faker.Image.LoremFlickrUrl());
            product.AddCategory(category);
            return product;
        }
    }
}
