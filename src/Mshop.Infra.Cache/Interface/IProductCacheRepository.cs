﻿using Mshop.Core.Cache;
using Mshop.Core.DomainObject;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Cache.Interface
{
    public interface IProductCacheRepository : ICacheRepository<Product>
    {
        /*Task<PaginatedOutPut<Product>>? FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken);

        Task<PaginatedOutPut<Product>>? FilterPaginatedByCategory(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken);*/

        Task<PaginatedOutPut<Product>>? FilterPaginatedQuery(PaginatedInPut input, Guid categoryId, bool promotion, CancellationToken cancellationToken);
    }

}
