using BMPTec.ChuBank.Api.DTOs;
using FluentValidation;

namespace BMPTec.ChuBank.Api.Validators
{
    public class TransferCreateDtoValidator : AbstractValidator<TransferCreateDto>
    {
        public TransferCreateDtoValidator()
        {
            RuleFor(x => x.FromAccountId).NotEmpty();
            RuleFor(x => x.ToAccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
