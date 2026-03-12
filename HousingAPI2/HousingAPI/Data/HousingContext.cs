using HousingAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HousingAPI.Data
{

    public class HousingContext : IdentityDbContext<ApplicationUser>
    {

        public HousingContext(DbContextOptions<HousingContext> options) : base(options) { }
        public DbSet<House> Houses { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Resident> Residents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Важно для Identity

            // Ваши кастомные конфигурации
            modelBuilder.Entity<House>()
                .HasMany(h => h.Apartments)
                .WithOne(a => a.House)
                .HasForeignKey(a => a.HouseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Apartment>()
                .HasMany(a => a.Residents)
                .WithOne(r => r.Apartment)
                .HasForeignKey(r => r.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resident>()
                .HasIndex(r => r.PersonalCode)
                .IsUnique();
        }
    }
}