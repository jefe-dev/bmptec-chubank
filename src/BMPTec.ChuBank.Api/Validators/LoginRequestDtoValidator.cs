using FluentValidation;
using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Validators
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MinimumLength(3)
                .WithMessage("Username must have at least 3 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must have at least 6 characters")
                .Must(password => !string.IsNullOrWhiteSpace(password))
                .WithMessage("Password cannot be empty")
                .Must(password => !password.Contains(" "))
                .WithMessage("Password cannot contain spaces")
                .Must(BusinessRuleValidator.IsSecurePassword)
                .WithMessage("Password must contain at least one letter and one number");

            RuleFor(x => x.Password)
                .Must(ValidatePasswordComplexity)
                .WithMessage("Password does not meet security requirements");
        }

        private bool ValidatePasswordComplexity(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.All(char.IsDigit) || password.All(char.IsLetter))
                return false;

            return true;
        }
    }
}
