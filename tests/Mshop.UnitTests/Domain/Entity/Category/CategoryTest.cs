using CoreMessage = Mshop.Core.Message;

namespace Mshop.UnitTests.Domain.Entity.Category
{
    public class CategoryTest : CategoryTestFixture, IDisposable
    {
        private readonly CoreMessage.Notifications _notifications;

        public CategoryTest()
        {
            _notifications = new CoreMessage.Notifications();
        }

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Business","Category")]
        public void Instantiate()
        {
            var valid = FakerCategory();

            var category = GetCategoryValid(valid.Name, valid.IsActive); ;
            category.IsValid(_notifications);

            Assert.False(_notifications.HasErrors());
            Assert.NotNull(category);
            Assert.Equal(valid.Name, category.Name);
            Assert.Equal(valid.IsActive, category.IsActive);
            Assert.NotEqual(Guid.Empty, category.Id);

        }

        [Theory(DisplayName = nameof(SholdReturnErroWhenNameIsInvalid))]
        [Trait("Business","Category")]
        [MemberData(nameof(ListNamesCategoryInvalid))]
        public void SholdReturnErroWhenNameIsInvalid(string? name)
        {
            var category = GetCategoryValid(name);
            //Action action = () => category.IsValid(_notifications);
            //var exception = Assert.Throws<EntityValidationException>(action);
            //Assert.Equal("Validation errors", exception.Message);
            
            Assert.False(category.IsValid(_notifications));            
            Assert.True(_notifications.HasErrors());

        }


        [Fact(DisplayName = nameof(SholdActivateCategory))]
        [Trait("Business","Category")]
        public void SholdActivateCategory()
        {

            var category = GetCategoryValid(FakerCategory().Name, false);
            category.Active();
            category.IsValid(_notifications);

            Assert.True(category.IsActive);
            Assert.False(_notifications.HasErrors());
        }

        [Fact(DisplayName = nameof(SholdDeactiveCategory))]
        [Trait("Business","Category")]
        public void SholdDeactiveCategory()
        {            
            var category = GetCategoryValid(FakerCategory().Name, true);
            category.Deactive();
            category.IsValid(_notifications);

            Assert.False(category.IsActive);
            Assert.False(_notifications.HasErrors());
        }


        [Fact(DisplayName = nameof(SholdUpdateCategory))]
        [Trait("Business","Category")]
        public void SholdUpdateCategory()
        {            
            var newValidade = new
            {
                Name = "Category New"
            };

            var category = FakerCategory();
            category.Update(newValidade.Name);
            category.IsValid(_notifications);

            Assert.False(_notifications.HasErrors());
            Assert.Equal(category.Name, newValidade.Name);
        }

        public void Dispose()
        {
            _notifications.Errors().Clear();
        }
    }
}
