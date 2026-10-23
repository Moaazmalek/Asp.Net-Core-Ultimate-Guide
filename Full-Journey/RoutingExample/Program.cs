using RoutingExample.CustomConstraints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("months", typeof(MonthsCustomConstraint));
});
var app = builder.Build();



// Routing is automatically enabled.
// No need for app.UseRouting() anymore


//Endpoints are defined directly on the "app" object 
//app.MapGet("/map1",async (context) =>
//{
//    await context.Response.WriteAsync("In Map 1");
//});
//app.Map("/map1",async (context) =>
//{
//    await context.Response.WriteAsync("In Map 1");
//});
//app.Map("/map2",async (context) =>
//{
//    await context.Response.WriteAsync("In Map 2");
//});



app.Map("products/details/{productId:int?}", async (HttpContext context, int productId) =>
{
    await context.Response.WriteAsync($"Details Of Product - [{productId}]");
});


//       files/sample.txt

app.Map("files/{filename}.{extension=txt}", async  ( HttpContext context, string filename, string extension) =>
{
    await context.Response.WriteAsync($"In Files {filename}.{extension}");
});

app.Map("employee/profile/{employeeName}", async (HttpContext context) =>
{
    string? employeeName = Convert.ToString(context.Request.RouteValues["employeeName"]);
    await context.Response.WriteAsync($"In Employee Profile - {employeeName}");

});

// daily-digest-report/{reportdate}

app.MapGet("daily-digest-report/{reportdate:datetime}", 
    async (HttpContext context, DateTime reportdate) => {
        await context.Response.WriteAsync($"{reportdate}");
});

app.Map("cities/{cityId:guid}", async (HttpContext context, Guid cityId) =>
{
    await context.Response.WriteAsync($"City information - {cityId}"); 

});

app.Map("sales-report/{year:int:min(1900)}/{month:months}", async (HttpContext context, int year, string month) =>
{
    await context.Response.WriteAsync($"sales report - {year} - {month}");
});
app.MapFallback(async context =>
{
    await context.Response.WriteAsync($"Request recieved at {context.Request.Path}");
});
app.Run();
