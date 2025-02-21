namespace Mshop.Application.Common
{
    public abstract class PaginatedListOutPut<TOutPutItens>
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<TOutPutItens> Data { get; set; }
        protected PaginatedListOutPut(int currentPage, int perPage, int total, IReadOnlyList<TOutPutItens> data)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
            Data = data;
        }

    }
}
