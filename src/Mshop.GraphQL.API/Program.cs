using Mshop.API.GraphQL.GraphQL.Category;
using Mshop.API.GraphQL.GraphQL.Product;
using Mshop.Application;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddUseCase()
    .AddCircuitOptions()
    .AddDataBaseAndRepository(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddRepositoryCache()
    //.AddConfigurationSeriLog(builder.Configuration)
    .AddGraphQLServer()
    .AddQueryType()
    .AddTypeExtension<CategoryQueries>()
    .AddTypeExtension<ProductQueries>();

builder.Host.UseSerilog();

var app = builder.Build();

//app.AddLayoutSerilog();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGraphQL();

app.MapControllers();

app.Run();

namespace Mshop.API.GraphQL
{
    public partial class Program
    { }
}
