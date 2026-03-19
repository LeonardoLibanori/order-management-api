using FluentValidation;
using OrderManagement.Application.DTOs.Company;

namespace OrderManagement.Application.Validators;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyDto>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da empresa é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.");

        RuleFor(x => x.Address)
            .MaximumLength(300).WithMessage("Endereço deve ter no máximo 300 caracteres.");
    }
}