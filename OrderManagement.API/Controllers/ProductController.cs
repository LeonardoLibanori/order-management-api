using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Product;
using OrderManagement.Application.Interfaces.Services;

namespace OrderManagement.API.Controllers;

public class ProductController : BaseController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Lista todos os produtos da empresa com paginação</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetAllAsync(GetUserId(), page, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Retorna um produto pelo ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Cria um novo produto</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Atualiza um produto existente</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Remove um produto (soft delete)</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}