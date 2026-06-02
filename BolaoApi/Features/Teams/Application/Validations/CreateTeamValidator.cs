using FluentValidation;
using BolaoApi.Features.Teams.Application.Dtos;

namespace BolaoApi.Features.Teams.Application.Validations;

public class CreateTeamValidator : AbstractValidator<CreateTeamRequestDto>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MinimumLength(3).WithMessage("Team name must have at least 3 characters")
            .MaximumLength(255).WithMessage("Team name must not exceed 255 characters");

        RuleFor(x => x.UrlFlag)
            .NotEmpty().WithMessage("Team flag URL is required")
            .Must(IsValidUrl).WithMessage("Team flag URL must be a valid URL");
    }

    private bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
