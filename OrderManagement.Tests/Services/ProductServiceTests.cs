using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs.Product;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace OrderManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _productService = new ProductService(
            _productRepositoryMock.Object,
            _companyRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateProduct_WithoutCompany_ShouldReturnFailure()
    {
        // Arrange
        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((Company?)null);

        var dto = new CreateProductDto
        {
            Name = "Produto",
            Price = 10,
            StockQuantity = 5,
            Category = "Cat"
        };

        // Act
        var result = await _productService.CreateAsync(dto, 1);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Cadastre uma empresa antes de adicionar produtos.");
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var company = new Company { Id = 1, UserId = 1 };
        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(company);
        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new CreateProductDto
        {
            Name = "Produto Teste",
            Description = "Descrição",
            Price = 29.90m,
            StockQuantity = 100,
            Category = "Eletrônicos"
        };

        // Act
        var result = await _productService.CreateAsync(dto, 1);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be(dto.Name);
        result.Data.Price.Should().Be(dto.Price);
    }

    [Fact]
    public async Task DeleteProduct_ShouldSoftDelete()
    {
        // Arrange
        var company = new Company { Id = 1, UserId = 1 };
        var product = new Product { Id = 1, CompanyId = 1, IsActive = true };

        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(company);
        _productRepositoryMock.Setup(r => r.GetByIdAndCompanyAsync(1, 1)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _productService.DeleteAsync(1, 1);

        // Assert
        result.Success.Should().BeTrue();
        product.IsActive.Should().BeFalse(); // confirma soft delete
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }
}