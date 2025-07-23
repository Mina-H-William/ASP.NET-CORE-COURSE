using Entities;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // not working in newer versions of Microsoft.EntityFrameworkCore.sql and Microsoft.EntityFrameworkCore
                // but work fine in older versions
                //var descripter = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                //if (descripter != null)
                //{
                //    services.Remove(descripter);
                //}

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });


            });
        }
    }
}
