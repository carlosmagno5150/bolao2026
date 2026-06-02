namespace BolaoApi.Features.Championships.Application.Dtos;

public record CreateChampionshipRequestDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate
);

public record CreateChampionshipResponseDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt
);
