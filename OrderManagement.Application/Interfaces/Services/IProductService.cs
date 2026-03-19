using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Product;

namespace OrderManagement.Application.Interfaces.Services;

public interface IProductService
{
    Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductDto dto, int userId);
    Task<ApiResponse<PagedResponse<ProductResponseDto>>> GetAllAsync(int userId, int page, int pageSize);
    Task<ApiResponse<ProductResponseDto>> GetByIdAsync(int productId, int userId);
    Task<ApiResponse<ProductResponseDto>> UpdateAsync(int productId, UpdateProductDto dto, int userId);
    Task<ApiResponse<bool>> DeleteAsync(int productId, int userId);
}