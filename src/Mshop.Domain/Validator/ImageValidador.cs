using Mshop.Core.DomainObject;
using Mshop.Core.Message;

namespace Mshop.Domain.Validator
{
    public class ImageValidador : Notification
    {
        private readonly Entity.Image _image;
        public ImageValidador(Entity.Image image, INotification notification) : base(notification)
        {
            _image = image;
        }

        public override INotification Validate()
        {
            ValidationDefault.NotNullOrEmpty(_image.FileName, nameof(_image.FileName), _notifications);

            ValidationDefault.NotNullGuid(_image.ProductId, nameof(_image.ProductId), _notifications);
            ValidationDefault.IsValidGuid(_image.ProductId.ToString(), nameof(_image.ProductId), _notifications);

            ValidationDefault.NotNullGuid(_image.Id, nameof(_image.Id), _notifications);
            ValidationDefault.IsValidGuid(_image.Id.ToString(), nameof(_image.Id), _notifications);

            return _notifications;
        }
    }
}
