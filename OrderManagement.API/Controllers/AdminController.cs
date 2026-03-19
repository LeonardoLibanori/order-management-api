using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Common;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Lista todos os usuários — apenas Admin</summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Role,
                u.CreatedAt,
                HasCompany = u.Company != null
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Ok(users));
    }

    /// <summary>Estatísticas gerais do sistema — apenas Admin</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalCompanies = await _context.Companies.CountAsync(),
            TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalRevenue = await _context.Orders
                .Where(o => o.Status == Domain.Enums.OrderStatus.Completed)
                .SumAsync(o => o.TotalAmount)
        };

        return Ok(ApiResponse<object>.Ok(stats));
    }

    /// <summary>Promove um usuário para Admin</summary>
    [HttpPatch("users/{id}/promote")]
    public async Task<IActionResult> PromoteToAdmin(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("Usuário não encontrado."));

        user.Role = "Admin";
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { user.Id, user.Name, user.Role }, "Usuário promovido a Admin."));
    }
}