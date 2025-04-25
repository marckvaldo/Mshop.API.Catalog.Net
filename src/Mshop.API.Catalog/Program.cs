using Microsoft.AspNetCore.Http;
using Mshop.API.Catalog.Configuration;
using Mshop.Application;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;
using Serilog;
using Serilog.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


builder.Services.AddConfigurationController()
    .AddConfigurationModelState()
    .AddCircuitOptions()
    .AddDataBaseAndRepository(builder.Configuration)
    .AddCache(builder.Configuration)
    .AddRepositoryCache()
    //.AddConfigurationSeriLog(builder.Configuration)
    .AddConfigurationHealthChecks()
    .AddUseCase();

    //builder.Host.UseSerilog();
    
    //builder.WebHost.UseUrls("http://*8080");

    //aqui e seu quieser usar o logs nativo do asp.net para ser coletados com o filebeat
    //.AddHttpLogging(opt =>
    //{
    //opt.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All; // Loga tudo
    //});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
var app = builder.Build();


//Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
//Console.WriteLine(Directory.GetCurrentDirectory());


//configurando o serilog para ler as requests
//app.AddLayoutSerilog();

//aqui eu ativo os logs nativo do asp.net para ser coletoado com o filebeat
//app.UseHttpLogging();

app.AddMigrateDatabase();
app.CrateIndexRedis();
app.UseDocumentation();
app.AddMapHealthCheck();
 
//Console.WriteLine($"Ambiente = {app.Configuration["EnvironmentCheck"]}");

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

namespace Mshop.API.Catalog
{
    public partial class Program
    {

    }
}
