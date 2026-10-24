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
app.UseSerilogRequestLogging(); //adds extra log message when the request response is completed
 
//app.UseHttpLogging();
if (builder.Environment.IsEnvironment("Test") ==false)
{
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}


app.UseStaticFiles();
app.MapControllers();


app.Run();


public partial class Program {} // make the auto-generated program accessible programmatically