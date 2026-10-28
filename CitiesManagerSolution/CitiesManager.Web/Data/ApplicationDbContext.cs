using CitiesManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<City>().HasData(new City[]
            {
                new()
                {
                    Id=Guid.Parse("129299A0-6F1D-40B4-AA18-01E4E8872CC9"),
                    Name="New York"
                },
                new()
                {
                    Id= Guid.Parse("87622BDC-CF8F-4AFD-BA5F-8FE8403FFFA3"),
                    Name="London"
                }
            });
        }
    }
}
