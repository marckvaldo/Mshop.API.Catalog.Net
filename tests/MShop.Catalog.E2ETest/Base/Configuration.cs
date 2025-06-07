namespace Mshop.Catalog.E2ETest.Base
{
    public static class Configuration
    {
        public static string NAME_DATA_BASE  = "end2end-test-db";
        public static string URL_API_PRODUCT = "/api/v1/products/";
        public static string URL_API_CATEGORY = "/api/v1/categories/";
        public static string URL_API_CACHE = "/api/v1/cache/";

        public static string URL_API_IMAGE = "/api/v1/images/";
        public static bool DATABASE_MEMORY = false;

        public static string URI_GRAPHQL = "http://localhost:5000/graphql";
    }
}

