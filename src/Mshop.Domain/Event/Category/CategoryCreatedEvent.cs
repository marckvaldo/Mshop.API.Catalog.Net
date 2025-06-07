using Mshop.Core.DomainObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event.Category
{
    public class CategoryCreatedEvent : DomainEvent
    {
        public Guid Id { get; private set; }
        public CategoryCreatedEvent(Guid id) : base()
        {
            Id = id;
        }
    }
}
