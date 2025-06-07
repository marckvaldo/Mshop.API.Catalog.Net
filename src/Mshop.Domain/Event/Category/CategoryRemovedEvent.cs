using Mshop.Core.DomainObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Event.Category
{
    public class CategoryRemovedEvent : DomainEvent
    {
        public Guid Id { get; private set; }
        public CategoryRemovedEvent(Guid id) : base()
        {
            Id = id;
        }
    }
}
