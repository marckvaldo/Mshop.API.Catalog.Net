using Mshop.Domain.Validator;
using CoreException = Mshop.Core.Exception;
using Core = Mshop.Core.Message;
using Mshop.Core.Message;

namespace Mshop.Domain.Entity
{
    public class Image : Core.DomainObject.Entity
    {
        public Image(string fileName, Guid productId) : base()
        {
            FileName = fileName;
            ProductId = productId;
        }

        public Image(string fileName, Guid id, Guid productId)
        {
            FileName = fileName;
            ProductId = productId;
            AddId(id);  
        }

        public string FileName { get; set; }
        public Guid ProductId { get; set; }

        /*public override void IsValid(Core.Message.INotification notification)
        {
            var imageValidate = new ImageValidador(this, notification);
            imageValidate.Validate();
            if (notification.HasErrors())
            {
                throw new CoreException.EntityValidationException("Validation errors");
            }
        }*/

        public override bool IsValid(INotification notification)
        {
            var productValidador = new ImageValidador(this, notification);
            productValidador.Validate();
            return !notification.HasErrors();

        }

        public void UpdateUrlImage(string fileName)
        {
            FileName = fileName;
        }

    }
}
