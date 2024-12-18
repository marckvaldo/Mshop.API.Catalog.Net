using Mshop.Application.UseCases.Image.Common;

namespace Mshop.Application.UseCases.Image.DelteImage
{
    public class DeleteImageTestFixute : ImageBaseFixtureTest
    {
        public Guid FakerRequest()
        {
            return Guid.NewGuid();
        }
    }
}
