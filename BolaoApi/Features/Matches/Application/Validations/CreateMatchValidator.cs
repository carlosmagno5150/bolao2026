using FluentValidation;
using BolaoApi.Features.Matches.Application.Dtos;

namespace BolaoApi.Features.Matches.Application.Validations;

public class CreateMatchValidator : AbstractValidator<CreateMatchRequestDto>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.ChampionshipId)
            .NotEmpty().WithMessage("Championship ID is required");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Match date is required")
            .GreaterThan(DateTime.MinValue).WithMessage("Match date must be valid");

        RuleFor(x => x.HomeTeamId)
            .NotEmpty().WithMessage("Home team ID is required");

        RuleFor(x => x.VisitorTeamId)
            .NotEmpty().WithMessage("Visitor team ID is required")
            .NotEqual(x => x.HomeTeamId).WithMessage("Home team and visitor team must be different");

        RuleFor(x => x.ScoreHomeTeam)
            .GreaterThanOrEqualTo(0).When(x => x.ScoreHomeTeam.HasValue)
            .WithMessage("Home team score must be greater than or equal to 0");

        RuleFor(x => x.ScoreVisitorTeam)
            .GreaterThanOrEqualTo(0).When(x => x.ScoreVisitorTeam.HasValue)
            .WithMessage("Visitor team score must be greater than or equal to 0");
    }
}
