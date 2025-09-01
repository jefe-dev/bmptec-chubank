using FluentValidation;
using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Validators
{
    public class TransferCreateDtoValidator : AbstractValidator<TransferCreateDto>
    {
        public TransferCreateDtoValidator()
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .WithMessage("Source account is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Source account is invalid");

            RuleFor(x => x.ToAccountId)
                .NotEmpty()
                .WithMessage("Destination account is required")
                .Must(id => id != Guid.Empty)
                .WithMessage("Destination account is invalid");

            RuleFor(x => x)
                .Must(x => x.FromAccountId != x.ToAccountId)
                .WithMessage("Cannot transfer to the same account");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");
        }
    }
}
