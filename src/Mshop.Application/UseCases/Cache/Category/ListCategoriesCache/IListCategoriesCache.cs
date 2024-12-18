using MediatR;
using Mshop.Core.DomainObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public interface IListCategoriesCache : IRequestHandler<ListCategoriesCacheInPut, Result<ListCategoriesCacheOutPut>>
    {

    }
}
