using Banking.Application.Requests.User;
using FluentValidation;

namespace Banking.Application.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<RegisterRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.Role)
                .IsInEnum();
        }
    }
}