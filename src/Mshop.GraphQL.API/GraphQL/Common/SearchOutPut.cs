namespace Mshop.API.GraphQL.GraphQL.Common
{
    public class SearchOutPut<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }

        public IReadOnlyList<T> Data { get; set; }

        public SearchOutPut(int currentPage, int perPage, int total, IReadOnlyList<T> data)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
            Data = data;
        }
    }
}
