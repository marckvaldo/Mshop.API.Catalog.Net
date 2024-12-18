using Mshop.Domain.Entity;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;

namespace Mshop.Domain.Validator
{
    public class ProductValidador : Notification
    {
        private readonly Product _product;

        public ProductValidador(Product product, INotification notification) : base(notification)
        {
            _product = product;
        }

        public override INotification Validate()
        {
            //Description
            ValidationDefault.NotNullOrEmpty(_product.Description, nameof(_product.Description), _notifications);
            ValidationDefault.MinLength(_product.Description, 10, nameof(_product.Description), _notifications);
            ValidationDefault.MaxLength(_product.Description, 1000, nameof(_product.Description), _notifications);

            //Name
            ValidationDefault.NotNullOrEmpty(_product.Name, nameof(_product.Name), _notifications);
            ValidationDefault.MinLength(_product.Name, 3, nameof(_product.Name), _notifications);
            ValidationDefault.MaxLength(_product.Name, 255, nameof(_product.Name), _notifications);

            //Price
            ValidationDefault.MustPositive(_product.Price, nameof(_product.Price), _notifications);
            ValidationDefault.MustBiggerOrEqualThan(_product.Price, 0.1M, nameof(_product.Price), _notifications);

            //stok
            //ValidationDefault.MustBiggerOrEqualThan(_product.Stock, 0_00, nameof(_product.Stock), _notifications);    

            return _notifications;

        }


    }
}
