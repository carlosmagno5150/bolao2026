namespace BolaoApi.Features.Matches.Application.Dtos;

public record CreateMatchRequestDto(
    Guid ChampionshipId,
    DateTime Date,
    Guid HomeTeamId,
    Guid VisitorTeamId,
    int? ScoreHomeTeam,
    int? ScoreVisitorTeam
);

public record CreateMatchResponseDto(
    Guid Id,
    Guid ChampionshipId,
    DateTime Date,
    Guid HomeTeamId,
    Guid VisitorTeamId,
    int? ScoreHomeTeam,
    int? ScoreVisitorTeam,
    DateTime CreatedAt
);
