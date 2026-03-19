using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Company;

namespace OrderManagement.Application.Interfaces.Services;

public interface ICompanyService
{
    Task<ApiResponse<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto, int userId);
    Task<ApiResponse<CompanyResponseDto>> GetByUserIdAsync(int userId);
    Task<ApiResponse<CompanyResponseDto>> UpdateAsync(UpdateCompanyDto dto, int userId);
    Task<ApiResponse<bool>> DeleteAsync(int userId);
}