using Banking.Application.Requests.Transaction;
using FluentValidation;

namespace Banking.Application.Validators.Transaction
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