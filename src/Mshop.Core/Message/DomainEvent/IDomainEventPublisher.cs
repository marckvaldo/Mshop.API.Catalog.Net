using CoreObject = Mshop.Core.DomainObject;

namespace Mshop.Core.Message.DomainEvent
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<TDomainEvent>(TDomainEvent entity) where TDomainEvent : CoreObject.DomainEvent;
    }
}
