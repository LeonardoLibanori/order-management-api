using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByUserIdAsync(int userId);
    Task<Company?> GetWithProductsAsync(int companyId);
}