using Banking.Application.Requests.Auth;
using FluentValidation;

namespace Banking.Application.Validators.Auth
{
    public class LoginRequestValidator
        : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(50)
                .WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must contain at least 8 characters.");
        }
    }
}