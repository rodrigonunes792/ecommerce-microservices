using Catalog.API.DTOs;
using Catalog.API.Models;

namespace Catalog.API.Extensions;

/// <summary>
/// Mapping extensions for DTOs and Entities
/// Implements SRP - handles only object mapping
/// </summary>
public static class MappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.ImageUrl,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.CreatedAt,
            product.UpdatedAt,
            product.IsActive
        );
    }

    public static Product ToEntity(this CreateProductDto dto, Guid? id = null)
    {
        return new Product
        {
            Id = id ?? Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static void UpdateFromDto(this Product product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.UpdatedAt = DateTime.UtcNow;
    }

    public static ProductListDto ToListDto(
        this IEnumerable<Product> products,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        var productDtos = products.Select(p => p.ToDto());
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new ProductListDto(
            productDtos,
            totalCount,
            pageNumber,
            pageSize,
            totalPages
        );
    }
}
