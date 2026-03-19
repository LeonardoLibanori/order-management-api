using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCompanyIdAsync(int companyId, int page, int pageSize)
        => await _dbSet
            .Where(p => p.CompanyId == companyId && p.IsActive)
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> CountByCompanyIdAsync(int companyId)
        => await _dbSet.CountAsync(p => p.CompanyId == companyId && p.IsActive);

    public async Task<Product?> GetByIdAndCompanyAsync(int productId, int companyId)
        => await _dbSet
            .FirstOrDefaultAsync(p => p.Id == productId && p.CompanyId == companyId && p.IsActive);
}