using Catalog.API.Models;

namespace Catalog.API.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, string? search, Guid? categoryId);
    Task<int> GetProductCountAsync(string? search, Guid? categoryId);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(CreateProductRequest request);
    Task UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task DeleteProductAsync(Guid id);
    Task UpdateStockAsync(Guid id, int quantity);
}
