using Core.Requests.Delete;
using FluentValidation;

namespace Core.Validators
{
    public class UsuarioDeleteRequestValidator : AbstractValidator<UsuarioDeleteRequest>
    {

        public UsuarioDeleteRequestValidator()
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage("O ID do usuário é obrigatório.")
                 .GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");
        }


    }
}
