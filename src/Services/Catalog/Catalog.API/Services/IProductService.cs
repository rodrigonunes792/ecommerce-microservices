using Catalog.API.Common;
using Catalog.API.DTOs;

namespace Catalog.API.Services;

/// <summary>
/// Product service interface using Result pattern
/// Implements ISP - interface segregated by functionality
/// Implements DIP - controllers depend on this abstraction
/// </summary>
public interface IProductService
{
    Task<Result<ProductListDto>> GetProductsAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateProductAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdateStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default);
}
