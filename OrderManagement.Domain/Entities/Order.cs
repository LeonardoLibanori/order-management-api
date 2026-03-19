using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    // FK
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    // Navigation property
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}