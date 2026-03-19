using FluentValidation;
using OrderManagement.Application.DTOs.Product;

namespace OrderManagement.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Categoria é obrigatória.")
            .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.");
    }
}