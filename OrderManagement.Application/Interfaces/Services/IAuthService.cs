using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Auth;

namespace OrderManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
}