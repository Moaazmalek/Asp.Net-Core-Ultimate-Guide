using ConfigurationExample.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("weatherapi"));


// Load MyOwnConfig.json
builder.Configuration.AddJsonFile("MyOwnConfig.json",optional:true);
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.Map("/config", async context =>
//    {
//        //await context.Response.WriteAsync(app.Configuration["MyKey"]);
//        await context.Response.WriteAsync(app.Configuration.GetValue<string>("MyKey");

//    });
//});
app.Run();
