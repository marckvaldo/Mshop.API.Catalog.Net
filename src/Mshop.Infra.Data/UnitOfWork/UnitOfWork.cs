using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Infra.Data.Context;
using Mshop.Core.Message.DomainEvent;

namespace Mshop.Infra.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RepositoryDbContext _repositoryDbContext;
        private readonly IDomainEventPublisher _publisher;

        public UnitOfWork(RepositoryDbContext repositoryDbContext, IDomainEventPublisher publisher)
        {
            _repositoryDbContext = repositoryDbContext;
            _publisher = publisher;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            //start to get events from domain
            var aggregateRoot = _repositoryDbContext.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Events.Any())
                .Select(x => x.Entity)
                .ToList();

            var events = aggregateRoot
                .SelectMany(x => x.Events)
                .ToList();

            //end to get events from domain

            await _repositoryDbContext.SaveChangesAsync(cancellationToken);

            foreach (var @event in events)
                await _publisher.PublishAsync((dynamic)@event);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
