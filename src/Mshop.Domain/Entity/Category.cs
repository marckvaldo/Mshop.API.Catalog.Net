using Mshop.Domain.Validator;
using Mshop.Core.Exception;
using Core = Mshop.Core.Message;


namespace Mshop.Domain.Entity
{
    public class Category : Core.DomainObject.Entity
    {

        public string Name { get; private set; }

        public bool IsActive { get; private set; }

        //Entity
        public List<Product> Products { get; private set; }

        public Category(string name, bool isActive = true) : base()
        {
            Name = name;
            IsActive = isActive;
            AddId(Guid.NewGuid());
        }

        public Category(string name, Guid id, bool isActive = true) : base()
        {
            Name = name;
            IsActive = isActive;
            AddId(id);  
        }

        public override bool IsValid(Core.Message.INotification notification)
        {
            var categoryValidador = new CategoryValidador(this, notification);
            categoryValidador.Validate();
            return !notification.HasErrors();
        }

        public void Active()
        {
            IsActive = true;
        }

        public void Deactive()
        {
            IsActive = false;
        }

        public void Update(string name)
        {
            Name = name;
        }
    }
}
