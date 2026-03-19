using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Company;
using OrderManagement.Application.Interfaces.Services;

namespace OrderManagement.API.Controllers;

public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>Cria uma nova empresa para o usuário logado</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
        var result = await _companyService.CreateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Retorna a empresa do usuário logado</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _companyService.GetByUserIdAsync(GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Atualiza os dados da empresa</summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyDto dto)
    {
        var result = await _companyService.UpdateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Remove a empresa do usuário logado</summary>
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var result = await _companyService.DeleteAsync(GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}