using FluentValidation;
using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Validators
{
    public class StatementQueryDtoValidator : AbstractValidator<StatementQueryDto>
    {
        public StatementQueryDtoValidator()
        {
            RuleFor(x => x.From)
                .NotEmpty()
                .WithMessage("Start date is required")
                .Must(date => date <= DateTime.Today)
                .WithMessage("Start date cannot be in the future");

            RuleFor(x => x.To)
                .NotEmpty()
                .WithMessage("End date is required")
                .Must(date => date <= DateTime.Today)
                .WithMessage("End date cannot be in the future");
        }
    }
}
