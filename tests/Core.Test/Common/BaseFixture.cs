using Bogus;
using Mshop.Application.Common;
using DomainEntity = Mshop.Domain.Entity;
using HelpersAplication = Mshop.Application.Common;

namespace Mshop.Core.Test.Common
{
    public class BaseFixture
    {
        protected Faker faker;
        public static Faker fakerStatic = new Faker("pt_BR");

        public BaseFixture()
        {
            faker = new Faker("pt_BR");
        }

        protected static FileInputBase64 ImageFake64()
        {
            return new FileInputBase64(FileFakerBase64.IMAGE64);
        }

        protected static string ExtensionFile(string file)
        {
            return HelpersAplication.Helpers.GetExtensionBase64(file);
        }

        public DomainEntity.Category FakerCategory()
        {
            return new DomainEntity.Category(GetNameCategoryValid());
        }

        protected string GetNameCategoryValid()
        {
            string category = faker.Commerce.Categories(1)[0];
            while (category.Length < 3)
            {
                category = faker.Commerce.Categories(1)[0];
            }

            if (category.Length > 30)
                category = category[..30];

            return category;
        }

        public List<DomainEntity.Image> FakerImages(Guid productId, int quantity = 3)
        {
            List<DomainEntity.Image> images = new();
            for (int i = 1; i <= quantity; i++)
                images.Add(new DomainEntity.Image(fakerStatic.Image.LoremFlickrUrl(), productId));

            return images;
        }

        public DomainEntity.Image FakerImage(Guid productId)
        {
            return new DomainEntity.Image(fakerStatic.Image.LoremFlickrUrl(), productId);
        }
    }
}
