using Bogus;
using Mshop.Application.Common;

namespace Mshop.UnitTests.Common
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
            return Helpers.GetExtensionBase64(file);
        }
    }
}
