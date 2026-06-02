namespace BolaoApi.Features.Teams.Application.Dtos;

public record CreateTeamRequestDto(
    string Name,
    string UrlFlag
);

public record CreateTeamResponseDto(
    Guid Id,
    string Name,
    string UrlFlag,
    DateTime CreatedAt
);
