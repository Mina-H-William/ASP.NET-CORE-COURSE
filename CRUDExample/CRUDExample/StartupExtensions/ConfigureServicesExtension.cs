using CRUDExample.Filters.ActionFilters;
using Entities;
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

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders |
                                        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
            });

            return services;
        }
    }
}
