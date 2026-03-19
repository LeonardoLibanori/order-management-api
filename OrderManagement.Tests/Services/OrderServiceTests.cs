using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs.Order;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace OrderManagement.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _companyRepositoryMock.Object);
    }

    // ─── Create ──────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;
        var company = new Company { Id = 1, UserId = userId, Name = "Empresa Teste" };
        var product = new Product { Id = 1, Name = "Produto", Price = 50, StockQuantity = 10, CompanyId = 1, IsActive = true };

        var dto = new CreateOrderDto
        {
            Notes = "Pedido teste",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };

        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(company);
        _productRepositoryMock.Setup(r => r.GetByIdAndCompanyAsync(1, 1)).ReturnsAsync(product);
        _orderRepositoryMock.Setup(r => r.GenerateOrderNumberAsync()).ReturnsAsync("ORD-20260319-0001");
        _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _orderRepositoryMock.Setup(r => r.GetWithItemsAsync(It.IsAny<int>())).ReturnsAsync(new Order
        {
            Id = 1,
            OrderNumber = "ORD-20260319-0001",
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            CompanyId = 1,
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, Product = product, Quantity = 2, UnitPrice = 50 }
            }
        });

        // Act
        var result = await _orderService.CreateAsync(dto, userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.TotalAmount.Should().Be(100);
        result.Data.OrderNumber.Should().Be("ORD-20260319-0001");
        result.Data.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyItems_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            Items = new List<CreateOrderItemDto>() // lista vazia
        };

        _companyRepositoryMock
            .Setup(r => r.GetByUserIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Company?)null); // empresa não encontrada

        // Act
        var result = await _orderService.CreateAsync(dto, userId: 1);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Empresa não encontrada.");
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1;
        var company = new Company { Id = 1, UserId = userId };
        var product = new Product { Id = 1, Name = "Produto", Price = 50, StockQuantity = 1, CompanyId = 1, IsActive = true };

        var dto = new CreateOrderDto
        {
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 5 } // pede 5, tem só 1
            }
        };

        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(company);
        _productRepositoryMock.Setup(r => r.GetByIdAndCompanyAsync(1, 1)).ReturnsAsync(product);
        _orderRepositoryMock.Setup(r => r.GenerateOrderNumberAsync()).ReturnsAsync("ORD-20260319-0001");

        // Act
        var result = await _orderService.CreateAsync(dto, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Estoque insuficiente");
    }

    // ─── UpdateStatus ─────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_CompletedOrder_ShouldDeductStock()
    {
        // Arrange
        var userId = 1;
        var company = new Company { Id = 1, UserId = userId };
        var product = new Product { Id = 1, StockQuantity = 10 };

        var order = new Order
        {
            Id = 1,
            CompanyId = 1,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, Product = product, Quantity = 3, UnitPrice = 50 }
            }
        };

        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(company);
        _orderRepositoryMock.Setup(r => r.GetWithItemsAsync(1)).ReturnsAsync(order);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new UpdateOrderStatusDto { Status = OrderStatus.Completed };

        // Act
        var result = await _orderService.UpdateStatusAsync(1, dto, userId);

        // Assert
        result.Success.Should().BeTrue();
        product.StockQuantity.Should().Be(7); // 10 - 3 = 7
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_CancelledOrder_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1;
        var company = new Company { Id = 1, UserId = userId };
        var order = new Order
        {
            Id = 1,
            CompanyId = 1,
            Status = OrderStatus.Cancelled
        };

        _companyRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(company);
        _orderRepositoryMock.Setup(r => r.GetWithItemsAsync(1)).ReturnsAsync(order);

        var dto = new UpdateOrderStatusDto { Status = OrderStatus.Completed };

        // Act
        var result = await _orderService.UpdateStatusAsync(1, dto, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Não é possível alterar um pedido cancelado.");
    }
}