using CitiesManager.Core.Entites;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any additional model configurations here

            modelBuilder.Entity<City>().HasData(new List<City>()
            {
                new City(){CityID = Guid.NewGuid(), CityName = "New York"},
                new City(){CityID = Guid.NewGuid(), CityName = "Egypt"},
            });
        }
    }
}
