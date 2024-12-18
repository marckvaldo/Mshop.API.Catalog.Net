
using Mshop.Core.DomainObject;
using Mshop.Core.Message;

namespace Mshop.Domain.ValueObject
{
    public class Dimensions
    {
        public decimal Height { get; set; } 

        public decimal Width { get; set; }

        public decimal Depth { get; set; }

        public Dimensions(decimal height, decimal width, decimal depth, INotification notification)
        {
            ValidationDefault.MustBiggerThan(height, 1, nameof(height), notification);
            ValidationDefault.MustBiggerThan(width, 1, nameof(width), notification);
            ValidationDefault.MustBiggerThan(depth, 1, nameof(depth), notification);

            Height = height;
            Width = width;
            Depth = depth;
        }

        public string Descrition()
        {
            return $"LxAxD: {Width} x {Height} x {Depth}";
        }

        public override string ToString()
        {
            return Descrition();
        }
    }
}
