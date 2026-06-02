using BolaoApi.Features.Matches.Application.Dtos;
using BolaoApi.Features.Matches.Application.Validations;
using BolaoApi.Features.Matches.Infrastructure.Interfaces;
using BolaoApi.Features.Matches.Domain.Entities;

namespace BolaoApi.Features.Matches.Application.Endpoints;

public static class CreateMatchEndpoint
{
    public static void MapCreateMatch(this WebApplication app)
    {
        app.MapPost("/matches", CreateMatch)
            .WithName("CreateMatch")
            .WithOpenApi()
            .WithSummary("Create a new match")
            .Produces<CreateMatchResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateMatch(
        CreateMatchRequestDto request,
        IMatchRepository matchRepository,
        ILogger<Program> logger)
    {
        try
        {
            var validator = new CreateMatchValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                logger.LogWarning("Match creation validation failed: {Errors}", string.Join(", ", errors));
                return Results.BadRequest(new { Errors = errors });
            }

            var match = new Match
            {
                ChampionshipId = request.ChampionshipId,
                Date = request.Date,
                HomeTeamId = request.HomeTeamId,
                VisitorTeamId = request.VisitorTeamId,
                ScoreHomeTeam = request.ScoreHomeTeam,
                ScoreVisitorTeam = request.ScoreVisitorTeam
            };

            var createdMatch = await matchRepository.CreateAsync(match);
            await matchRepository.SaveChangesAsync();

            var response = new CreateMatchResponseDto(
                createdMatch.Id,
                createdMatch.ChampionshipId,
                createdMatch.Date,
                createdMatch.HomeTeamId,
                createdMatch.VisitorTeamId,
                createdMatch.ScoreHomeTeam,
                createdMatch.ScoreVisitorTeam,
                createdMatch.CreatedAt
            );

            logger.LogInformation("Match created successfully: {MatchId}", createdMatch.Id);
            return Results.Created($"/matches/{createdMatch.Id}", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating match");
            return Results.Problem("An error occurred while creating the match", statusCode: 500);
        }
    }
}
