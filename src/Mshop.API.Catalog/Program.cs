using Mshop.API.Catalog.Configuration;
using Mshop.Application;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddConfigurationController()
    .AddConfigurationModelState()
    .AddDataBaseAndRepository(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddRepositoryCache()
    .AddUseCase();

var app = builder.Build();

app.AddMigrateDatabase();
app.CrateIndexRedis();
app.UseDocumentation();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }
