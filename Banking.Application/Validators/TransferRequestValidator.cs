using Banking.Application.Requests.Transaction;
using FluentValidation;

namespace Banking.Application.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .WithMessage("Source account is required.");

            RuleFor(x => x.ToAccountId)
                .NotEmpty()
                .WithMessage("Destination account is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(200);
        }
    }
}