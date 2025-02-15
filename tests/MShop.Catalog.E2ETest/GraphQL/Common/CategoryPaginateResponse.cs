using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.GraphQL.Common
{
    public class CategoryPaginateResponse
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public ListCategories ListCategories { get; set; }
    }

    public class ListCategories
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public List<Item> Itens { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
