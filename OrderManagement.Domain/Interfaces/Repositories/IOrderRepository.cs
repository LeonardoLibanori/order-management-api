using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Interfaces.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetByCompanyIdAsync(int companyId, int page, int pageSize);
    Task<Order?> GetWithItemsAsync(int orderId);
    Task<int> CountByCompanyIdAsync(int companyId);
    Task<string> GenerateOrderNumberAsync();
}