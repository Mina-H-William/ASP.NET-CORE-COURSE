using ContactsManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration,
             IWebHostEnvironment environment)
        {
            services.AddTransient<ResponseHeaderActionFilter>();

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5); // can't supply parameters to the filter here but order work fine

                // GetRequiredService for throw exception if the service is not registered
                // GetService will return null if the service is not registered
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                { key = "My-Key-From-Global", value = "My-Value-From-Global", Order = 2 });


                // Add global filter to validate anti-forgery token for all POST requests
                // if we use validateantiforgerytoken it will apply for all requests but GET requests should never modify data,
                // so making CSRF protection unnecessary for get requests,so AutoValidateAntiforgeryTokenAttribute recommended
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<ICountriesService, CountriesService>();

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            // filters to use in serviceFilter Attribute
            services.AddTransient<PersonsListActionFilter>();

            if (!environment.IsEnvironment("Test"))
                services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 5; // minimum password length
                options.Password.RequireNonAlphanumeric = false; // allow passwords without special characters
                options.Password.RequireUppercase = false; // allow passwords without uppercase letters
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false; // allow passwords without digits
                options.Password.RequiredUniqueChars = 3; // minimum number of unique characters in the password
            })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders() // for password reset, email confirmation, etc.
                 .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                 .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
            // not required to explicitly add Userstore and Rolestore as they are added by default
            // unless we want to use custom UserStore or RoleStore
            //AddIdentity() Automatically Calls AddAuthentication()

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                // enforces authorization for all endpoints by default
                options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return !context.User.Identity?.IsAuthenticated ?? false;
                    });
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                // this the default value for pathes that are used by cookie authentication
                // /Account/{action} action can be Login, Logout, AccessDenied, etc.
                // so if you didn't specify these paths, they will be used by default
                options.LoginPath = "/Account/Login"; // redirect to login page if user is not authenticated
                options.AccessDeniedPath = "/Error"; // redirect to access denied page if user is authenticated but not authorized
            });

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders |
                                        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
            });

            return services;
        }
    }
}
