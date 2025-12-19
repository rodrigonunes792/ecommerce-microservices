using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var electronicsId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();
        var booksId = Guid.NewGuid();

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = electronicsId, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = clothingId, Name = "Clothing", Description = "Fashion and apparel" },
            new Category { Id = booksId, Name = "Books", Description = "Books and publications" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop Pro 15\"",
                Description = "High-performance laptop with 16GB RAM and 512GB SSD",
                Price = 1299.99m,
                StockQuantity = 50,
                ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853",
                CategoryId = electronicsId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with precision tracking",
                Price = 29.99m,
                StockQuantity = 200,
                ImageUrl = "https://images.unsplash.com/photo-1527814050087-3793815479db",
                CategoryId = electronicsId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Cotton T-Shirt",
                Description = "Comfortable 100% cotton t-shirt",
                Price = 19.99m,
                StockQuantity = 150,
                ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab",
                CategoryId = clothingId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Clean Code",
                Description = "A Handbook of Agile Software Craftsmanship by Robert C. Martin",
                Price = 39.99m,
                StockQuantity = 75,
                ImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765",
                CategoryId = booksId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}
