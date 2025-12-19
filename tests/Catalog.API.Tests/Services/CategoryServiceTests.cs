using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests.Services;

public class CategoryServiceTests : IDisposable
{
    private readonly CatalogDbContext _context;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CatalogDbContext(options);
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _categoryService = new CategoryService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category1 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Electronic devices and accessories"
        };

        var category2 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Books",
            Description = "Books and publications"
        };

        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                StockQuantity = 50,
                ImageUrl = "laptop.jpg",
                CategoryId = category1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Clean Code",
                Description = "Programming book",
                Price = 39.99m,
                StockQuantity = 100,
                ImageUrl = "book.jpg",
                CategoryId = category2.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Old Mouse",
                Description = "Discontinued product",
                Price = 19.99m,
                StockQuantity = 0,
                ImageUrl = "mouse.jpg",
                CategoryId = category1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = false // Inactive product
            }
        };

        _context.Categories.AddRange(category1, category2);
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldIncludeOnlyActiveProducts()
    {
        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        var electronicsCategory = result.First(c => c.Name == "Electronics");
        var activeProducts = electronicsCategory.Products.Where(p => p.IsActive).ToList();
        activeProducts.Should().HaveCount(1); // Only active products
        activeProducts.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        var category = _context.Categories.First();

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ShouldIncludeProducts()
    {
        // Arrange
        var category = _context.Categories.First(c => c.Name == "Electronics");

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Products.Should().NotBeEmpty();
        var activeProducts = result.Products.Where(p => p.IsActive).ToList();
        activeProducts.Should().NotBeEmpty();
        activeProducts.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _categoryService.GetCategoryByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnCategoriesWithCorrectProductCount()
    {
        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        var electronicsCategory = result.First(c => c.Name == "Electronics");
        var booksCategory = result.First(c => c.Name == "Books");

        var activeElectronicsProducts = electronicsCategory.Products.Where(p => p.IsActive).ToList();
        var activeBooksProducts = booksCategory.Products.Where(p => p.IsActive).ToList();

        activeElectronicsProducts.Should().HaveCount(1); // Only 1 active product
        activeBooksProducts.Should().HaveCount(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
