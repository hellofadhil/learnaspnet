using Microsoft.EntityFrameworkCore;
using MyProject.Models;

namespace MyProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurasi default untuk UUID
            modelBuilder
                .Entity<ProductCategory>()
                .Property(p => p.Id)
                .HasDefaultValueSql("gen_random_uuid()");
        }
    }
}
