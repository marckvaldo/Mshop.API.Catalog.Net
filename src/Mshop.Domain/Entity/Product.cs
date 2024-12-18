using Mshop.Domain.Validator;
using Mshop.Domain.ValueObject;
using Mshop.Core.DomainObject;
using CoreException = Mshop.Core.Exception;
using Mshop.Core.Message;

namespace Mshop.Domain.Entity
{
    public class Product : Core.DomainObject.Entity, IAggregateRoot
    {
        public string Description { get; private set; }

        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public decimal Stock { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsSale { get; private set; }

        public Guid CategoryId { get; private set; }

        //public Dimensions Dimensions { get; private set; }

        //Entity
        public Category Category { get; private set; }

        public FileImage? Thumb { get; private set; }  

        public Product(string description, string name, decimal price, Guid categoryId, decimal stock = 0, bool isActive = false) : base()
        {
            Description = description;
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = isActive;
            CategoryId = categoryId;
        }

        public Product(string description, string name, decimal price, Guid categoryId, Guid id, decimal stock = 0, bool isActive = false) : base()
        {
            Description = description;
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = isActive;
            CategoryId = categoryId;
            AddId(id);
        }

        /*public override void IsValid(INotification notification)
        {
            var productValidador = new ProductValidador(this, notification);
            productValidador.Validate();
            if (notification.HasErrors())
            {
                throw new CoreException.EntityValidationException("Validation errors");
            }

        }*/

        public override bool IsValid(INotification notification)
        {
            var productValidador = new ProductValidador(this, notification);
            productValidador.Validate();
            return !notification.HasErrors();
            
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactive()
        {
            IsActive = false;
        }

        public void Update(string description, string name, decimal price, Guid categoryId)
        {
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
        }

        public void AddQuantityStock(decimal stock)
        {
            Stock += stock;
        }

        public void RemoveQuantityStock(decimal stock)
        {
            if(stock < 0) stock *= -1;
            Stock -= stock;
        }

        public void UpdateQuantityStock(decimal stock)
        {
            Stock = stock;
        }

        public void UpdateThumb(string thumb)
        {
            Thumb = new FileImage(thumb);
        }

        public void ActivateSale()
        {
            IsSale = true;
        }

        public void DeactiveSale()
        {
            IsSale = false;
        }

        public void AddCategory(Category category)
        {
            Category = category;
            CategoryId = category.Id;
        }

    }
}
