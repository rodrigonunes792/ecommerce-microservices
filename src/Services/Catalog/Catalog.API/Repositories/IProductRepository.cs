using Catalog.API.Models;

namespace Catalog.API.Repositories;

/// <summary>
/// Product-specific repository interface
/// Implements ISP (Interface Segregation Principle) - specific interface for Product operations
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsWithCategoryAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsProductNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
