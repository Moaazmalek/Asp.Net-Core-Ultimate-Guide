using CRUDExample.Filters.ActionFilters; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.Infrastructure.Data;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            //services.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            //{
            //    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
            //    .ReadFrom.Services(services);
            //});
            //Enable Identity in this project
            services.
                //Configure the Identity management services
                AddIdentity<ApplicationUser, ApplicationRole>((options) =>
                {
                    options.Password.RequiredLength = 5;
                    options.Password.RequireNonAlphanumeric = false; //special character is mandatory or not
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 3;// should be minimum of 3 unique characters
                })
                //It configures Entity Framework Core as the persistence mechanism for Asp.Net Core Identity
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                //offers the repository layer
                .AddUserStore<UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext,Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole,ApplicationDbContext,Guid>>();
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//enfources authorization policy (user must be authenticated) for all the action methods
                options.AddPolicy("NotAuthenticated", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        //return true if the user notauthenticated right now 
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });   
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;
        }
    }
}
