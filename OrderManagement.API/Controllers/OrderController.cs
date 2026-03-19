using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Order;
using OrderManagement.Application.Interfaces.Services;

namespace OrderManagement.API.Controllers;

public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Lista todos os pedidos da empresa com paginação</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _orderService.GetAllAsync(GetUserId(), page, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Retorna um pedido pelo ID com seus itens</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Cria um novo pedido</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await _orderService.CreateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Atualiza o status de um pedido</summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await _orderService.UpdateStatusAsync(id, dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Cancela um pedido</summary>
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _orderService.CancelAsync(id, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}