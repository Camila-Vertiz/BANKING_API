using Banking.Application.Request.Customer;
using FluentValidation;

namespace Banking.Application.Validators
{

    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("Document type is required.")
                .IsInEnum().WithMessage("Invalid document type.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithMessage("Document number is required.")
                .MaximumLength(20)
                .WithMessage("Document number cannot exceed 20 characters.");

            RuleFor(x => x.DocumentNumber)
                .Matches(@"^\d+$")
                .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber))
                .WithMessage("Document number must be numeric.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
