var builder = WebApplication.CreateBuilder(args);
// Register Services
builder.Services.AddControllersWithViews();


var app = builder.Build();
// Use Services
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.MapControllers();


app.Run();
