using Mshop.Application;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;
using Mshop.gRPC.Catalog.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddDataBaseAndRepository(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddCircuitOptions()
    .AddRepositoryCache()
    //.AddConfigurationSeriLog(builder.Configuration)
    .AddUseCase()
    .AddGrpc();

builder.Host.UseSerilog();

var app = builder.Build();

//app.AddLayoutSerilog();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ProductService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

namespace Mshop.gRPC.Catalog
{
    public partial class Program
    { }
}