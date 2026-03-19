using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Company;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto, int userId)
    {

        // Verifica se usuário já tem empresa
        var existing = await _companyRepository.GetByUserIdAsync(userId);
        if (existing is not null)
            return ApiResponse<CompanyResponseDto>.Fail("Usuário já possui uma empresa cadastrada.");

        var company = new Company
        {
            Name = dto.Name.Trim(),
            Document = dto.Document.Trim(),
            Phone = dto.Phone.Trim(),
            Address = dto.Address.Trim(),
            UserId = userId
        };

        await _companyRepository.AddAsync(company);
        await _companyRepository.SaveChangesAsync();

        return ApiResponse<CompanyResponseDto>.Ok(MapToDto(company), "Empresa criada com sucesso.");
    }

    public async Task<ApiResponse<CompanyResponseDto>> GetByUserIdAsync(int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company is null)
            return ApiResponse<CompanyResponseDto>.Fail("Empresa não encontrada.");

        return ApiResponse<CompanyResponseDto>.Ok(MapToDto(company));
    }

    public async Task<ApiResponse<CompanyResponseDto>> UpdateAsync(UpdateCompanyDto dto, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company is null)
            return ApiResponse<CompanyResponseDto>.Fail("Empresa não encontrada.");

        company.Name = dto.Name.Trim();
        company.Phone = dto.Phone.Trim();
        company.Address = dto.Address.Trim();

        await _companyRepository.UpdateAsync(company);
        await _companyRepository.SaveChangesAsync();

        return ApiResponse<CompanyResponseDto>.Ok(MapToDto(company), "Empresa atualizada com sucesso.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);

        if (company is null)
            return ApiResponse<bool>.Fail("Empresa não encontrada.");

        await _companyRepository.DeleteAsync(company);
        await _companyRepository.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Empresa removida com sucesso.");
    }

    private static CompanyResponseDto MapToDto(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Document = company.Document,
        Phone = company.Phone,
        Address = company.Address,
        CreatedAt = company.CreatedAt
    };
}