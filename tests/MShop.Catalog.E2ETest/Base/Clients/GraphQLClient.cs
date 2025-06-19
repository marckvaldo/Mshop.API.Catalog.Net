using Mshop.Catalog.E2ETest.Base;
using System.Text;
using System.Text.Json;

namespace Mshop.Catalog.E2ETests.Base.Clients
{
    public class GraphQLClient
    {
        protected HttpClient _grahQLClient;
        public GraphQLClient(HttpClient grahQLClient)
        {
            _grahQLClient = grahQLClient;
        }

        public async Task<T> SendQuery<T>(string query)
        {
            var queryGraphQL = new
            {
                query
            };

            var content = new StringContent(JsonSerializer.Serialize(queryGraphQL), Encoding.UTF8, "application/json");

            var response = await _grahQLClient.PostAsync(Configuration.URI_GRAPHQL, content);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, options);
        }


    }
}
