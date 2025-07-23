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
                // not working dont know why
                // i Think it work with sqlserver package older than i used
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
