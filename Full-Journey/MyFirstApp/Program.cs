var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.Run(async(HttpContext context) =>
{
    context.Response.StatusCode = 200;

    await context.Response.WriteAsync("Hello");
    await context.Response.WriteAsync(" World");

});
app.Run();
