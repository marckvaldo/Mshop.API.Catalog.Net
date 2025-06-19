using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mshop.Catalog.E2ETests.Base.FactoriesWeb
{
    public class CustomerWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            var environment = "E2ETests";
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment); //setando globalmente
            builder.UseEnvironment(environment); // apenas no host da aplicação

            /*builder.ConfigureServices(services =>
            {
                services.AddRepositoryCache()
                        .AddDataBaseAndRepository()
            });*/

            base.ConfigureWebHost(builder);
        }
    }
}
