using PayloadCategory = Mshop.Catalog.E2ETests.GraphQL.Common.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mshop.Catalog.E2ETests.GraphQL.Common.Product
{
    public class ProductByIdResponse
    {
        public ProductByIdResponseData Data { get; set; }
    }

    public class ProductByIdResponseData
    {
        public Product ProductById { get; set; }
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PayloadCategory.Category Category { get; set; }
        public string CategoryId { get; set; }
        public decimal Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsPromotion { get; set; }
        public string Thumb { get; set; }
        public string Description { get; set; }
    }

    /*public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }*/

}
