using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs.Order;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICompanyRepository _companyRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICompanyRepository companyRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<OrderResponseDto>> CreateAsync(CreateOrderDto dto, int userId)
    {

        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<OrderResponseDto>.Fail("Empresa não encontrada.");

        var order = new Order
        {
            OrderNumber = await _orderRepository.GenerateOrderNumberAsync(),
            CompanyId = company.Id,
            Notes = dto.Notes,
            Status = OrderStatus.Pending
        };

        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepository.GetByIdAndCompanyAsync(itemDto.ProductId, company.Id);

            if (product is null)
                return ApiResponse<OrderResponseDto>.Fail($"Produto {itemDto.ProductId} não encontrado.");

            if (itemDto.Quantity <= 0)
                return ApiResponse<OrderResponseDto>.Fail($"Quantidade inválida para o produto {product.Name}.");

            if (product.StockQuantity < itemDto.Quantity)
                return ApiResponse<OrderResponseDto>.Fail($"Estoque insuficiente para o produto {product.Name}. Disponível: {product.StockQuantity}.");

            var item = new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price // Captura o preço atual
            };

            order.Items.Add(item);
            total += item.Quantity * item.UnitPrice;
        }

        order.TotalAmount = total;

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        var created = await _orderRepository.GetWithItemsAsync(order.Id);
        return ApiResponse<OrderResponseDto>.Ok(MapToDto(created!), "Pedido criado com sucesso.");
    }

    public async Task<ApiResponse<PagedResponse<OrderResponseDto>>> GetAllAsync(int userId, int page, int pageSize)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<PagedResponse<OrderResponseDto>>.Fail("Empresa não encontrada.");

        var orders = await _orderRepository.GetByCompanyIdAsync(company.Id, page, pageSize);
        var total = await _orderRepository.CountByCompanyIdAsync(company.Id);

        var response = PagedResponse<OrderResponseDto>.Create(
            orders.Select(MapToDto),
            total, page, pageSize);

        return ApiResponse<PagedResponse<OrderResponseDto>>.Ok(response);
    }

    public async Task<ApiResponse<OrderResponseDto>> GetByIdAsync(int orderId, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<OrderResponseDto>.Fail("Empresa não encontrada.");

        var order = await _orderRepository.GetWithItemsAsync(orderId);

        if (order is null || order.CompanyId != company.Id)
            return ApiResponse<OrderResponseDto>.Fail("Pedido não encontrado.");

        return ApiResponse<OrderResponseDto>.Ok(MapToDto(order));
    }

    public async Task<ApiResponse<OrderResponseDto>> UpdateStatusAsync(int orderId, UpdateOrderStatusDto dto, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<OrderResponseDto>.Fail("Empresa não encontrada.");

        var order = await _orderRepository.GetWithItemsAsync(orderId);

        if (order is null || order.CompanyId != company.Id)
            return ApiResponse<OrderResponseDto>.Fail("Pedido não encontrado.");

        if (order.Status == OrderStatus.Cancelled)
            return ApiResponse<OrderResponseDto>.Fail("Não é possível alterar um pedido cancelado.");

        if (order.Status == OrderStatus.Completed)
            return ApiResponse<OrderResponseDto>.Fail("Não é possível alterar um pedido já finalizado.");

        // Regra de negócio: ao finalizar, deduz estoque
        if (dto.Status == OrderStatus.Completed)
        {
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product is null) continue;

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        order.Status = dto.Status;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return ApiResponse<OrderResponseDto>.Ok(MapToDto(order), "Status atualizado com sucesso.");
    }

    public async Task<ApiResponse<bool>> CancelAsync(int orderId, int userId)
    {
        var company = await _companyRepository.GetByUserIdAsync(userId);
        if (company is null)
            return ApiResponse<bool>.Fail("Empresa não encontrada.");

        var order = await _orderRepository.GetWithItemsAsync(orderId);

        if (order is null || order.CompanyId != company.Id)
            return ApiResponse<bool>.Fail("Pedido não encontrado.");

        if (order.Status == OrderStatus.Completed)
            return ApiResponse<bool>.Fail("Não é possível cancelar um pedido já finalizado.");

        if (order.Status == OrderStatus.Cancelled)
            return ApiResponse<bool>.Fail("Pedido já está cancelado.");

        order.Status = OrderStatus.Cancelled;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Pedido cancelado com sucesso.");
    }

    private static OrderResponseDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status.ToString(),
        TotalAmount = order.TotalAmount,
        Notes = order.Notes,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemResponseDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Subtotal = i.Subtotal
        }).ToList()
    };
}