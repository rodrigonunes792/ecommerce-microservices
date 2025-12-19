using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly CatalogDbContext _context;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;
    private readonly Guid _categoryId;

    public ProductServiceTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CatalogDbContext(options);
        _loggerMock = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_context, _loggerMock.Object);

        // Seed test data
        _categoryId = Guid.NewGuid();
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category
        {
            Id = _categoryId,
            Name = "Electronics",
            Description = "Electronic devices"
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
                CategoryId = _categoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                StockQuantity = 200,
                ImageUrl = "mouse.jpg",
                CategoryId = _categoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 89.99m,
                StockQuantity = 0,
                ImageUrl = "keyboard.jpg",
                CategoryId = _categoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = false // Inactive product
            }
        };

        _context.Categories.Add(category);
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnActiveProducts()
    {
        // Act
        var result = await _productService.GetProductsAsync(1, 10, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Only active products
        result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetProductsAsync_WithSearch_ShouldFilterProducts()
    {
        // Act
        var result = await _productService.GetProductsAsync(1, 10, "Laptop", null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetProductsAsync_WithCategoryFilter_ShouldReturnFilteredProducts()
    {
        // Act
        var result = await _productService.GetProductsAsync(1, 10, null, _categoryId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.CategoryId.Should().Be(_categoryId));
    }

    [Fact]
    public async Task GetProductsAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        var result = await _productService.GetProductsAsync(1, 1, null, null);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetProductCountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var count = await _productService.GetProductCountAsync(null, null);

        // Assert
        count.Should().Be(2); // Only active products
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var product = _context.Products.First(p => p.IsActive);

        // Act
        var result = await _productService.GetProductByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _productService.GetProductByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "Test product",
            Price = 99.99m,
            StockQuantity = 10,
            ImageUrl = "test.jpg",
            CategoryId = _categoryId
        };

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Price.Should().Be(request.Price);
        result.IsActive.Should().BeTrue();

        // Verify it was saved to database
        var savedProduct = await _context.Products.FindAsync(result.Id);
        savedProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidCategory_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "Test product",
            Price = 99.99m,
            StockQuantity = 10,
            ImageUrl = "test.jpg",
            CategoryId = Guid.NewGuid() // Non-existent category
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _productService.CreateProductAsync(request)
        );
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = _context.Products.First(p => p.IsActive);
        var request = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 199.99m,
            ImageUrl = "updated.jpg"
        };

        // Act
        await _productService.UpdateProductAsync(product.Id, request);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be(request.Name);
        updatedProduct.Description.Should().Be(request.Description);
        updatedProduct.Price.Should().Be(request.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 199.99m,
            ImageUrl = "updated.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _productService.UpdateProductAsync(Guid.NewGuid(), request)
        );
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldSoftDeleteProduct()
    {
        // Arrange
        var product = _context.Products.First(p => p.IsActive);

        // Act
        await _productService.DeleteProductAsync(product.Id);

        // Assert
        var deletedProduct = await _context.Products.FindAsync(product.Id);
        deletedProduct.Should().NotBeNull();
        deletedProduct!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateStockAsync_WithValidQuantity_ShouldUpdateStock()
    {
        // Arrange
        var product = _context.Products.First(p => p.IsActive);
        var initialStock = product.StockQuantity;
        var quantityToAdd = 10;

        // Act
        await _productService.UpdateStockAsync(product.Id, quantityToAdd);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.StockQuantity.Should().Be(initialStock + quantityToAdd);
    }

    [Fact]
    public async Task UpdateStockAsync_WithNegativeQuantityExceedingStock_ShouldThrowException()
    {
        // Arrange
        var product = _context.Products.First(p => p.IsActive);
        var quantityToRemove = -(product.StockQuantity + 1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _productService.UpdateStockAsync(product.Id, quantityToRemove)
        );
    }

    [Fact]
    public async Task UpdateStockAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _productService.UpdateStockAsync(Guid.NewGuid(), 10)
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
