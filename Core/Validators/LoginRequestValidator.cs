using Core.Requests.Create;
using FluentValidation;

namespace Core.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {

        public LoginRequestValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.")
                .MaximumLength(100).WithMessage("O e-mail deve ter no máximo 100 caracteres.");

            RuleFor(c => c.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MaximumLength(100).WithMessage("A senha deve ter no máximo 100 caracteres.");
        }

    }
}
