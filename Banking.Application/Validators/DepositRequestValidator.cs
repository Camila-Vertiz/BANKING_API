using Banking.Application.Requests.Transaction;
using FluentValidation;

namespace Banking.Application.Validators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("AccountId is required.");


            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");


            RuleFor(x => x.Description)
                .MaximumLength(250);
        }
    }
}