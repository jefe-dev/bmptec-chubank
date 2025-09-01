using FluentValidation;
using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Validators
{
    public class AccountCreateDtoValidator : AbstractValidator<AccountCreateDto>
    {
        public AccountCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(200)
                .WithMessage("Name must have a maximum of 200 characters")
                .Must(BusinessRuleValidator.IsValidName)
                .WithMessage("Name must contain only valid letters and spaces");

            RuleFor(x => x.CPF)
                .NotEmpty()
                .WithMessage("CPF is required")
                .Length(11)
                .WithMessage("CPF must have exactly 11 digits")
                .Matches(@"^\d+$")
                .WithMessage("CPF must contain only numbers")
                .Must(BusinessRuleValidator.IsValidCpf)
                .WithMessage("Invalid CPF");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Initial balance cannot be negative");
        }
    }
}
