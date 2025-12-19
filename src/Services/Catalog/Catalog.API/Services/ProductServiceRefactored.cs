using Catalog.API.Common;
using Catalog.API.DTOs;
using Catalog.API.Extensions;
using Catalog.API.Repositories;
using Catalog.API.Validators;

namespace Catalog.API.Services;

/// <summary>
/// Refactored Product Service following SOLID principles
///
/// SOLID Implementation:
/// - SRP: Only orchestrates business logic, delegates data access to Repository
/// - OCP: Extensible through validators and Result pattern
/// - LSP: Implements IProductService contract properly
/// - ISP: Interface is focused on product operations only
/// - DIP: Depends on IUnitOfWork abstraction, not concrete DbContext
/// </summary>
public class ProductServiceRefactored : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductServiceRefactored> _logger;
    private readonly CreateProductValidator _createValidator;
    private readonly UpdateProductValidator _updateValidator;

    public ProductServiceRefactored(
        IUnitOfWork unitOfWork,
        ILogger<ProductServiceRefactored> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _createValidator = new CreateProductValidator();
        _updateValidator = new UpdateProductValidator();
    }

    public async Task<Result<ProductListDto>> GetProductsAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.GetProductsWithCategoryAsync(
                pageNumber,
                pageSize,
                search,
                categoryId,
                cancellationToken);

            var totalCount = await _unitOfWork.Products.CountAsync(
                p => p.IsActive &&
                     (string.IsNullOrWhiteSpace(search) || p.Name.Contains(search) || p.Description.Contains(search)) &&
                     (!categoryId.HasValue || p.CategoryId == categoryId.Value),
                cancellationToken);

            var listDto = products.ToListDto(totalCount, pageNumber, pageSize);

            return Result<ProductListDto>.Success(listDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return Result<ProductListDto>.Failure("An error occurred while retrieving products");
        }
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id, cancellationToken);

            if (product == null)
            {
                return Result<ProductDto>.Failure($"Product with ID {id} not found");
            }

            return Result<ProductDto>.Success(product.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return Result<ProductDto>.Failure("An error occurred while retrieving the product");
        }
    }

    public async Task<Result<ProductDto>> CreateProductAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validation (OCP - validation is separated and extensible)
            var validationErrors = _createValidator.Validate(dto);
            if (validationErrors.Any())
            {
                return Result<ProductDto>.Failure(validationErrors);
            }

            // Business rule: Check if product name is unique
            var isUnique = await _unitOfWork.Products.IsProductNameUniqueAsync(dto.Name, cancellationToken: cancellationToken);
            if (!isUnique)
            {
                return Result<ProductDto>.Failure($"A product with the name '{dto.Name}' already exists");
            }

            // Create entity
            var product = dto.ToEntity();

            // Save
            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Fetch with category
            var createdProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id, cancellationToken);

            _logger.LogInformation("Product created: {ProductId} - {ProductName}", product.Id, product.Name);

            return Result<ProductDto>.Success(createdProduct!.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<ProductDto>.Failure("An error occurred while creating the product");
        }
    }

    public async Task<Result> UpdateProductAsync(
        Guid id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validation
            var validationErrors = _updateValidator.Validate(dto);
            if (validationErrors.Any())
            {
                return Result.Failure(validationErrors);
            }

            // Get product
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return Result.Failure($"Product with ID {id} not found");
            }

            // Business rule: Check if new name is unique (excluding current product)
            if (product.Name != dto.Name)
            {
                var isUnique = await _unitOfWork.Products.IsProductNameUniqueAsync(
                    dto.Name,
                    id,
                    cancellationToken);

                if (!isUnique)
                {
                    return Result.Failure($"A product with the name '{dto.Name}' already exists");
                }
            }

            // Update
            product.UpdateFromDto(dto);
            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product updated: {ProductId}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return Result.Failure("An error occurred while updating the product");
        }
    }

    public async Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return Result.Failure($"Product with ID {id} not found");
            }

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product deleted (soft): {ProductId}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return Result.Failure("An error occurred while deleting the product");
        }
    }

    public async Task<Result> UpdateStockAsync(
        Guid id,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return Result.Failure($"Product with ID {id} not found");
            }

            // Business rule: Cannot have negative stock
            if (product.StockQuantity + quantity < 0)
            {
                return Result.Failure("Insufficient stock. Operation would result in negative stock quantity");
            }

            product.StockQuantity += quantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product stock updated: {ProductId}, Quantity Change: {Quantity}, New Stock: {Stock}",
                id,
                quantity,
                product.StockQuantity);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", id);
            return Result.Failure("An error occurred while updating stock");
        }
    }
}
