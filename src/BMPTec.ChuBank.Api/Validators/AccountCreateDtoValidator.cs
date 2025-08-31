using BMPTec.ChuBank.Api.DTOs;
using FluentValidation;

namespace BMPTec.ChuBank.Api.Validators
{
    public class AccountCreateDtoValidator : AbstractValidator<AccountCreateDto>
    {
        public AccountCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CPF).NotEmpty().Length(11);
        }
    }
}
