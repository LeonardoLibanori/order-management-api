using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Order;

namespace OrderManagement.Application.Interfaces.Services;

public interface IOrderService
{
    Task<ApiResponse<OrderResponseDto>> CreateAsync(CreateOrderDto dto, int userId);
    Task<ApiResponse<PagedResponse<OrderResponseDto>>> GetAllAsync(int userId, int page, int pageSize);
    Task<ApiResponse<OrderResponseDto>> GetByIdAsync(int orderId, int userId);
    Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(int orderId, UpdateOrderStatusDto dto, int userId);
    Task<ApiResponse<bool>> CancelAsync(int orderId, int userId);
}