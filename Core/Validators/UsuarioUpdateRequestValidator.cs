using Core.Requests.Update;
using FluentValidation;

namespace Core.Validators
{
    public class UsuarioUpdateRequestValidator : AbstractValidator<UsuarioUpdateRequest>
    {

        public UsuarioUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do usuário é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");

        }

    }
}
