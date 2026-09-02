using MiddlewareExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<MyCustomMiddleware>();

var app = builder.Build();

//app.Run(async(HttpContext context) =>
//{
//    await context.Response.WriteAsync("Hello");


//});


// middleware 1
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("FROM MIDDLEWARE [1] \n");
    await next(context);

});

// middleware 2
//app.Run(async(HttpContext context) =>
//{
//    await context.Response.WriteAsync("Hello again");

//});

//app.UseMiddleware<MyCustomMiddleware>();
//app.UseMyCustomMiddleware();
app.UseHelloCustomMiddleware();

app.Run(async (context) =>
{
    await context.Response.WriteAsync("FROM MIDDLEWARE [3]\n");
});


app.Run();
