using Estoque.Api.Contracts;
using FluentValidation;

namespace Estoque.Api.Validators;

public class SupplierUpsertRequestValidator : AbstractValidator<SupplierUpsertRequest>
{
    public SupplierUpsertRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Nome do contato é obrigatório.")
            .MaximumLength(120).WithMessage("Nome do contato deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .MaximumLength(120).WithMessage("Email deve ter no máximo 120 caracteres.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório.")
            .MaximumLength(40).WithMessage("Telefone deve ter no máximo 40 caracteres.")
            .Matches(@"^[\d\s\-\(\)\+]+$").WithMessage("Telefone inválido.");
    }
}