using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs.Order;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}