using Microsoft.EntityFrameworkCore;
using MyProject.Models;

namespace MyProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurasi ProductCategory
            modelBuilder
                .Entity<ProductCategory>()
                .Property(p => p.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Konfigurasi Product
            modelBuilder
                .Entity<Product>()
                .Property(p => p.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Relasi Product ke ProductCategory
            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Atau Cascade jika ingin hapus otomatis
        }
    }
}
