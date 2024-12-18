
using Mshop.Core.Enum.Paginated;

namespace Mshop.Core.Paginated
{
    public class PaginatedInPut
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string Search { get; set; }

        public string OrderBy { get; set; }

        public SearchOrder Order { get; set; }

        //(1-1)*10=0
        //(2-1)*10=10
        //(3-1)*10=20
        public int From => (Page - 1) * PerPage;

        public PaginatedInPut(int page = 1, int perPage = 20, string search = "", string orderBy = "", SearchOrder order = SearchOrder.Asc)
        {
            Page = page;
            PerPage = perPage;
            Search = search;
            OrderBy = orderBy;
            Order = order;
        }



    }
}
