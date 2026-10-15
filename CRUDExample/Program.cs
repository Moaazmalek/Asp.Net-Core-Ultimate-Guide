using Entities;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);
// Register Services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
 
var app = builder.Build();
// Use Services
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
if (builder.Environment.IsEnvironment("Test") ==false)
{
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.MapControllers();


app.Run();


public partial class Program {} // make the auto-generated program accessible programmatically