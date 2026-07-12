using Banking.Application.Requests.BankAccount;
using FluentValidation;

namespace Banking.Application.Validators
{
    public class CreateBankAccountRequestValidator
        : AbstractValidator<CreateBankAccountRequest>
    {
        public CreateBankAccountRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");


            RuleFor(x => x.Currency)
                .IsInEnum()
                .WithMessage("Currency is invalid.");
        }
    }
}