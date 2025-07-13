using Core.Requests.Create;
using FluentValidation;

namespace Core.Validators
{
    public class UsuarioRequestValidator : AbstractValidator<UsuarioRequest>
    {

        public UsuarioRequestValidator()
        {
            RuleFor(regiao => regiao.Nome)
              .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O e-mail deve ter no máximo 100 caracteres.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.")
                .MaximumLength(100).WithMessage("O e-mail deve ter no máximo 100 caracteres.");

            RuleFor(regiao => regiao.Senha)
              .NotEmpty().WithMessage("A senha é obrigatória.")
                .MaximumLength(100).WithMessage("A senha deve ter no máximo 100 caracteres.");
                

            RuleFor(regiao => regiao.Role)
              .NotEmpty().WithMessage("A role é obrigatória.")
                .MaximumLength(50).WithMessage("A role deve ter no máximo 50 caracteres.");

        }

    }
}
