using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.GraphQL.Common.Product
{
    public class ProductPaginateResponse
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public listProduct ListProduct { get; set; }
    }

    public class listProduct
    {
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public List<Product> Data { get; set; }
    }


}
