namespace OrderManagement.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Propriedade calculada — não persiste no banco
    public decimal Subtotal => Quantity * UnitPrice;

    // FKs
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}