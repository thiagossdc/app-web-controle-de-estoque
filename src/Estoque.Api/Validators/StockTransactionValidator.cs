using Estoque.Api.Contracts;
using FluentValidation;

namespace Estoque.Api.Validators;

public class StockTransactionRequestValidator : AbstractValidator<StockTransactionRequest>
{
    public StockTransactionRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Produto é obrigatório.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de movimentação inválido.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");

        RuleFor(x => x.Reason)
            .MaximumLength(300).WithMessage("Motivo deve ter no máximo 300 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}