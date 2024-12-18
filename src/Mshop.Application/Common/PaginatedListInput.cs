
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using System.Security.Cryptography;

namespace Mshop.Application.Common
{
    public abstract class PaginatedListInput
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string Search { get;set; }

        public string Sort { get;set;}

        public SearchOrder Dir { get; set; }
        protected PaginatedListInput(int page, int perPage, string search, string sort = "", SearchOrder dir = SearchOrder.Asc)
        {
            Page = page;
            PerPage = perPage;
            Search = search;
            Sort = sort;
            Dir = dir;
        }

        public static PaginatedInPut FromPaginateListInput(PaginatedListInput paginate)
        {
            return new PaginatedInPut(paginate.Page, paginate.PerPage, paginate.Search, paginate.Sort,paginate.Dir);
        }
    }
}
