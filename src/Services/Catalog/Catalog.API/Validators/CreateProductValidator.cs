using Catalog.API.DTOs;

namespace Catalog.API.Validators;

/// <summary>
/// Product creation validator
/// Implements SRP - only validates product creation rules
/// Implements OCP - validation rules can be extended without modifying service code
/// </summary>
public class CreateProductValidator
{
    public List<string> Validate(CreateProductDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Product name is required");
        else if (dto.Name.Length > 200)
            errors.Add("Product name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(dto.Description))
            errors.Add("Product description is required");
        else if (dto.Description.Length > 1000)
            errors.Add("Product description cannot exceed 1000 characters");

        if (dto.Price <= 0)
            errors.Add("Price must be greater than zero");

        if (dto.Price > 999999.99m)
            errors.Add("Price cannot exceed 999999.99");

        if (dto.StockQuantity < 0)
            errors.Add("Stock quantity cannot be negative");

        if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && dto.ImageUrl.Length > 500)
            errors.Add("Image URL cannot exceed 500 characters");

        if (dto.CategoryId == Guid.Empty)
            errors.Add("Category ID is required");

        return errors;
    }
}

/// <summary>
/// Product update validator
/// </summary>
public class UpdateProductValidator
{
    public List<string> Validate(UpdateProductDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Product name is required");
        else if (dto.Name.Length > 200)
            errors.Add("Product name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(dto.Description))
            errors.Add("Product description is required");
        else if (dto.Description.Length > 1000)
            errors.Add("Product description cannot exceed 1000 characters");

        if (dto.Price <= 0)
            errors.Add("Price must be greater than zero");

        if (dto.Price > 999999.99m)
            errors.Add("Price cannot exceed 999999.99");

        if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && dto.ImageUrl.Length > 500)
            errors.Add("Image URL cannot exceed 500 characters");

        return errors;
    }
}
