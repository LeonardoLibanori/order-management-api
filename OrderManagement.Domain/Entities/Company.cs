namespace OrderManagement.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty; // CNPJ
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // FK para User (dono da empresa)
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties (1:N)
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}