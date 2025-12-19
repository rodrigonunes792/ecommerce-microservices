namespace Catalog.API.DTOs;

/// <summary>
/// Product Data Transfer Objects
/// Separates domain entities from API contracts (SRP + OCP)
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ImageUrl,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ImageUrl,
    Guid CategoryId
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl
);

public record UpdateStockDto(
    int Quantity
);

public record ProductListDto(
    IEnumerable<ProductDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);
