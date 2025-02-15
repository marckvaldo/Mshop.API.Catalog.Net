using Mshop.API.GraphQL.GraphQL.Category;
using Mshop.API.GraphQL.GraphQL.Product;
using Mshop.Application;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;

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
    .AddGraphQLServer()
    .AddQueryType()
    .AddTypeExtension<CategoryQueries>()
    .AddTypeExtension<ProductQueries>();



var app = builder.Build();

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
