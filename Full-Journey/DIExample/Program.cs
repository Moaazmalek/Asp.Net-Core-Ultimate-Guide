using DIExampleServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<ICitiesService, CitiesService>();
//builder.Services.AddTransient<ICitiesService, CitiesService>();
builder.Services.AddSingleton<ICitiesService, CitiesService>();
var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();

app.Run();
