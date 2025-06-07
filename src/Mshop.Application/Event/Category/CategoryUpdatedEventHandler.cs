using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Contract.Services;
using Mshop.Domain.Event.Category;

namespace Mshop.Application.Event.Category
{
    public class CategoryUpdatedEventHandler : IDomainEventHandler<CategoryUpdatedEvent>
    {
        private readonly IServiceCacheCategory _serviceCacheCategory;

        public CategoryUpdatedEventHandler(IServiceCacheCategory serviceCacheCategory)
        {
            _serviceCacheCategory = serviceCacheCategory;
        }

        public async Task<bool> HandlerAsync(CategoryUpdatedEvent domainEvent)
        {
            if (domainEvent is null)
            {
                Console.WriteLine(nameof(domainEvent), "Domain event cannot be null");
                return false;
            }
            
            if (domainEvent.Id == Guid.Empty)
            {
                Console.WriteLine("Category ID cannot be empty", nameof(domainEvent.Id));
                return false;
            }

            await _serviceCacheCategory.UpdateCategory(domainEvent.Id, CancellationToken.None);
            return true;
        }
    }
}
