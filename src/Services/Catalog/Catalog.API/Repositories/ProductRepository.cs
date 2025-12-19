using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories;

/// <summary>
/// Product repository implementation with specific queries
/// Implements SRP - handles only Product data access concerns
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsWithCategoryAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> IsProductNameUniqueAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.Name == name && p.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
