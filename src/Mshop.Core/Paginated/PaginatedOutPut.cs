using Mshop.Core.DomainObject;

namespace Mshop.Core.Paginated
{
    public class PaginatedOutPut<TEntity> where TEntity : Entity
    { 
        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyList<TEntity> Itens { get; set; }

        public PaginatedOutPut(int currentPage, int perPage, int total, IReadOnlyList<TEntity> itens)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
            Itens = itens;
            TotalPages = (int)(total / perPage);
        }


    }
}
