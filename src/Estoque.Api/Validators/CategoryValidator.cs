using Estoque.Api.Contracts;
using FluentValidation;

namespace Estoque.Api.Validators;

public class CategoryUpsertRequestValidator : AbstractValidator<CategoryUpsertRequest>
{
    public CategoryUpsertRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Descrição deve ter no máximo 300 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}