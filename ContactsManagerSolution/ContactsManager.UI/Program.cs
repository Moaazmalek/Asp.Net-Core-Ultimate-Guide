using CRUDExample;
using CRUDExample.Middleware;
using Serilog; 
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

//Serilog 
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); // read out current app's services and make them available to serilog
});


var app = builder.Build();

//create application pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging(); //adds extra log message when the request response is completed
 
//app.UseHttpLogging();
if (builder.Environment.IsEnvironment("Test") ==false)
{
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}


app.UseStaticFiles();
//Identifies the matching endpoint for an incoming request
app.UseRouting();
//reada the cookie and extract the name and identity 
app.UseAuthentication();
app.UseAuthorization();//validates access permissions of the user 
//also called endpoint middleware
app.MapControllers();//registers endpoints for attribute-routed controllers
app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(name: "default", pattern: "{controller}/{action}/{id?}");//registers a route pattern for conventional MVC routing
app.Run();


public partial class Program {} // make the auto-generated program accessible programmatically