using FluentValidation;
using BolaoApi.Features.Championships.Application.Dtos;

namespace BolaoApi.Features.Championships.Application.Validations;

public class CreateChampionshipValidator : AbstractValidator<CreateChampionshipRequestDto>
{
    public CreateChampionshipValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Championship name is required")
            .MinimumLength(3).WithMessage("Championship name must have at least 3 characters")
            .MaximumLength(255).WithMessage("Championship name must not exceed 255 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThan(DateTime.MinValue).WithMessage("Start date must be valid");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(DateTime.MinValue).WithMessage("End date must be valid")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");
    }
}
