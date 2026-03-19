using FluentValidation;
using OrderManagement.Application.DTOs.Company;

namespace OrderManagement.Application.Validators;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da empresa é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Matches(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$")
            .WithMessage("CNPJ deve estar no formato 00.000.000/0000-00.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.");

        RuleFor(x => x.Address)
            .MaximumLength(300).WithMessage("Endereço deve ter no máximo 300 caracteres.");
    }
}