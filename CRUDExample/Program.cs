using Entities.ServiceContracts;
using Entities.Services;

var builder = WebApplication.CreateBuilder(args);
// Register Services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

var app = builder.Build();
// Use Services
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.MapControllers();


app.Run();
