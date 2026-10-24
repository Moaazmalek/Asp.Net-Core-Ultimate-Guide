using CRUDExample.Filters.ActionFilters; 
using Microsoft.EntityFrameworkCore;
 
using Microsoft.AspNetCore.HttpLogging;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.Infrastructure.Data;
namespace CRUDExample 
{
    public static class ConfigureServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsService, PersonsService>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Register Services
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new ResponseHeaderActionFilter("My-Key-From-Global", "My-Value-From-Global", 2));
            });
            //services.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            //{
            //    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
            //    .ReadFrom.Services(services);
            //});
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;
        }
    }
}
