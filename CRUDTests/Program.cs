using Entities;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);



//Logging
//builder.Logging.ClearProviders()
//    .AddConsole()
//    .AddDebug();
//builder.Services.AddHttpLogging(options =>
//{
//    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
//});
//Serilog 
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services);
});
// Register Services
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<ResponseHeaderActionFilter>();
    //var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    options.Filters.Add(new TypeFilterAttribute(typeof(ResponseHeaderActionFilter)) { Arguments = ["My-Key-From-Global", "My-Value-From-Global"] });
    //options.Filters.Add(new ResponseHeaderActionFilter(logger,"My-Key-From-Global","My-Value-From-Global"));
});
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
 
var app = builder.Build();
app.UseSerilogRequestLogging(); //adds extra log message when the request response is completed
// Use Services
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//app.UseHttpLogging();
if (builder.Environment.IsEnvironment("Test") ==false)
{
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}


app.UseStaticFiles();
app.MapControllers();


app.Run();


public partial class Program {} // make the auto-generated program accessible programmatically