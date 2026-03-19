using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> GetByCompanyIdAsync(int companyId, int page, int pageSize);
    Task<int> CountByCompanyIdAsync(int companyId);
    Task<Product?> GetByIdAndCompanyAsync(int productId, int companyId);
}