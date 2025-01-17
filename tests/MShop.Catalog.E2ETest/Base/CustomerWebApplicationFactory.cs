using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Catalog.E2ETests.Base
{
    public class CustomerWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            var environment = "E2ETests";
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment); //setando globalmente
            builder.UseEnvironment(environment); // apenas no host

            /*builder.ConfigureServices(services =>
            {
                services.AddRepositoryCache()
                        .AddDataBaseAndRepository()
            });*/

            base.ConfigureWebHost(builder);
        }
    }
}
