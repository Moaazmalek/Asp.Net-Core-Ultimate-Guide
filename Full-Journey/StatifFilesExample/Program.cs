using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles(); // works with the web root path wwwroot
app.UseStaticFiles(new StaticFileOptions() { 
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath,"mywebroot"))
}); // works with "mywebroot"


app.Run();
