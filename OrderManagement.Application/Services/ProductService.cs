using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Product;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;

    public ProductService(IProductRepository productRepository, ICompanyRepository companyRepository)
    {
        _productRepository = productRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductDto dto, int userId)
    {

        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<ProductResponseDto>.Fail("Cadastre uma empresa antes de adicionar produtos.");

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category.Trim(),
            CompanyId = company.Id
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return ApiResponse<ProductResponseDto>.Ok(MapToDto(product), "Produto criado com sucesso.");
    }

    public async Task<ApiResponse<PagedResponse<ProductResponseDto>>> GetAllAsync(int userId, int page, int pageSize)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<PagedResponse<ProductResponseDto>>.Fail("Empresa não encontrada.");

        var products = await _productRepository.GetByCompanyIdAsync(company.Id, page, pageSize);
        var total = await _productRepository.CountByCompanyIdAsync(company.Id);

        var response = PagedResponse<ProductResponseDto>.Create(
            products.Select(MapToDto),
            total, page, pageSize);

        return ApiResponse<PagedResponse<ProductResponseDto>>.Ok(response);
    }

    public async Task<ApiResponse<ProductResponseDto>> GetByIdAsync(int productId, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<ProductResponseDto>.Fail("Empresa não encontrada.");

        var product = await _productRepository.GetByIdAndCompanyAsync(productId, company.Id);
        if (product is null)
            return ApiResponse<ProductResponseDto>.Fail("Produto não encontrado.");

        return ApiResponse<ProductResponseDto>.Ok(MapToDto(product));
    }

    public async Task<ApiResponse<ProductResponseDto>> UpdateAsync(int productId, UpdateProductDto dto, int userId)
    {

        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<ProductResponseDto>.Fail("Empresa não encontrada.");

        var product = await _productRepository.GetByIdAndCompanyAsync(productId, company.Id);
        if (product is null)
            return ApiResponse<ProductResponseDto>.Fail("Produto não encontrado.");

        product.Name = dto.Name.Trim();
        product.Description = dto.Description.Trim();
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.Category = dto.Category.Trim();

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return ApiResponse<ProductResponseDto>.Ok(MapToDto(product), "Produto atualizado com sucesso.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int productId, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<bool>.Fail("Empresa não encontrada.");

        var product = await _productRepository.GetByIdAndCompanyAsync(productId, company.Id);
        if (product is null)
            return ApiResponse<bool>.Fail("Produto não encontrado.");

        // Soft delete — apenas marca como inativo
        product.IsActive = false;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Produto removido com sucesso.");
    }

    private static ProductResponseDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        StockQuantity = product.StockQuantity,
        Category = product.Category,
        IsActive = product.IsActive,
        CreatedAt = product.CreatedAt
    };
}