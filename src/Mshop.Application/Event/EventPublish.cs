using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;

namespace Mshop.Application.Event
{
    public class EventPublish : IDomainEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventPublish(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> PublishAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : DomainEvent
        {
            var handlerType = _serviceProvider.GetService<IDomainEventHandler<TDomainEvent>>();

            if (handlerType is null)
            {
                Console.WriteLine($"Handler not found in {@event}");
                return false;
            }

            if (!await handlerType.HandlerAsync(@event))
            {
                Console.WriteLine($"Error in handler {handlerType.GetType().Name} for event {@event}");
                return false;
            }

            return true;
        }

        public async Task<bool> PublishAsync<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : DomainEvent
        {
            var result = true;

            foreach (var @event in events)
            {
                var handlerType = _serviceProvider.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType()));

                if (handlerType is null || !((IEnumerable<object>)handlerType).Any())
                {
                    Console.WriteLine($"Handler not found for {@event.GetType().Name}");
                    continue;
                }

                foreach (var handler in (IEnumerable<object>)handlerType)
                {
                    var method = handler.GetType().GetMethod("HandlerAsync");
                    if (method is null)
                        continue;
                    
                    var handlerResult = (Task<bool>)method.Invoke(handler, new object[] { @event });
                    if (!await handlerResult)
                    {
                        Console.WriteLine($"Error in handler {handler.GetType().Name} for event {@event}");
                        result = false;
                    } 
                }
            }

            return result;
        }
    }
}
