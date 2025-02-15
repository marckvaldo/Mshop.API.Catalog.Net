using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.GraphQL.Common
{
    public class CategoryByIdResponse
    {
        public CategoryByIdData Data { get; set; }
    }

    public class CategoryByIdData
    {
        public Category CategoryById { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}
