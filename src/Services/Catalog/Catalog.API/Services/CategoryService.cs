using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class CategoryService : ICategoryService
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(CatalogDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .AsSplitQuery()
            .ToListAsync();

        // Load only active products for each category
        foreach (var category in categories)
        {
            await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.IsActive)
                .LoadAsync();
        }

        return categories;
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category != null)
        {
            await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.IsActive)
                .LoadAsync();
        }

        return category;
    }
}
