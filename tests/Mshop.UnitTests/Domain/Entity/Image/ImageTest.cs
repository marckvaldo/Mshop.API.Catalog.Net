using NRedisStack.Search;
using CoreMessage = Mshop.Core.Message;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.UnitTests.Domain.Entity.Image
{
    public class ImageTest : ImageTestFixture, IDisposable
    {
        private readonly CoreMessage.Notifications _notifications;

        public ImageTest()
        {
            _notifications = new CoreMessage.Notifications();
        }

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Business", "Image")]

        public void Instantiate()
        {
            var productId = Guid.NewGuid();
            var fileName = FakerImage(productId).FileName;
            DomainEntity.Image imageEntity = new DomainEntity.Image(fileName, productId);
            imageEntity.IsValid(_notifications);

            Assert.NotNull(imageEntity);
            Assert.Equal(imageEntity.ProductId, productId);
            Assert.Equal(imageEntity.FileName, fileName);
            Assert.NotEqual(imageEntity.Id, Guid.Empty);
        }


        [Theory(DisplayName = nameof(ShoudReturErroInstantiate))]
        [Trait("Business", "Image")]
        [InlineData(null,"")]
        [InlineData(null, "image")]
        [InlineData(null,null)]
        public void ShoudReturErroInstantiate(Guid productId, string fileName)
        {
            DomainEntity.Image imageEntity = new DomainEntity.Image(fileName, productId);

            //Action action = () => image.IsValid(_notifications);
            //var exception = Assert.Throws<EntityValidationException>(action);   

            Assert.False(imageEntity.IsValid(_notifications));
            Assert.True(_notifications.HasErrors());
        }

        public void Dispose()
        {
            _notifications.Errors().Clear();
        }
    }
}
