using BolaoApi.Features.Teams.Application.Dtos;
using BolaoApi.Features.Teams.Application.Validations;
using BolaoApi.Features.Teams.Infrastructure.Interfaces;
using BolaoApi.Features.Teams.Domain.Entities;

namespace BolaoApi.Features.Teams.Application.Endpoints;

public static class CreateTeamEndpoint
{
    public static void MapCreateTeam(this WebApplication app)
    {
        app.MapPost("/teams", CreateTeam)
            .WithName("CreateTeam")
            .WithOpenApi()
            .WithSummary("Create a new team")
            .Produces<CreateTeamResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateTeam(
        CreateTeamRequestDto request,
        ITeamRepository teamRepository,
        ILogger<Program> logger)
    {
        try
        {
            var validator = new CreateTeamValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                logger.LogWarning("Team creation validation failed: {Errors}", string.Join(", ", errors));
                return Results.BadRequest(new { Errors = errors });
            }

            var team = new Team
            {
                Name = request.Name,
                UrlFlag = request.UrlFlag
            };

            var createdTeam = await teamRepository.CreateAsync(team);
            await teamRepository.SaveChangesAsync();

            var response = new CreateTeamResponseDto(
                createdTeam.Id,
                createdTeam.Name,
                createdTeam.UrlFlag,
                createdTeam.CreatedAt
            );

            logger.LogInformation("Team created successfully: {TeamId}", createdTeam.Id);
            return Results.Created($"/teams/{createdTeam.Id}", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating team");
            return Results.Problem("An error occurred while creating the team", statusCode: 500);
        }
    }
}
