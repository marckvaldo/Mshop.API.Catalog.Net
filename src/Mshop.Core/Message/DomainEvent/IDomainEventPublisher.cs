using CoreObject = Mshop.Core.DomainObject;

namespace Mshop.Core.Message.DomainEvent
{
    public interface IDomainEventPublisher
    {
        Task<bool> PublishAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : CoreObject.DomainEvent;

        Task<bool> PublishAsync<TDomainEvent>(IEnumerable<TDomainEvent> @event) where TDomainEvent : CoreObject.DomainEvent;
    }
}
