using BolaoApi.Features.Championships.Application.Dtos;
using BolaoApi.Features.Championships.Application.Validations;
using BolaoApi.Features.Championships.Infrastructure.Interfaces;
using BolaoApi.Features.Championships.Domain.Entities;

namespace BolaoApi.Features.Championships.Application.Endpoints;

public static class CreateChampionshipEndpoint
{
    public static void MapCreateChampionship(this WebApplication app)
    {
        app.MapPost("/championships", CreateChampionship)
            .WithName("CreateChampionship")
            .WithOpenApi()
            .WithSummary("Create a new championship")
            .Produces<CreateChampionshipResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateChampionship(
        CreateChampionshipRequestDto request,
        IChampionshipRepository championshipRepository,
        ILogger<Program> logger)
    {
        try
        {
            var validator = new CreateChampionshipValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                logger.LogWarning("Championship creation validation failed: {Errors}", string.Join(", ", errors));
                return Results.BadRequest(new { Errors = errors });
            }

            var championship = new Championship
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var createdChampionship = await championshipRepository.CreateAsync(championship);
            await championshipRepository.SaveChangesAsync();

            var response = new CreateChampionshipResponseDto(
                createdChampionship.Id,
                createdChampionship.Name,
                createdChampionship.StartDate,
                createdChampionship.EndDate,
                createdChampionship.CreatedAt
            );

            logger.LogInformation("Championship created successfully: {ChampionshipId}", createdChampionship.Id);
            return Results.Created($"/championships/{createdChampionship.Id}", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating championship");
            return Results.Problem("An error occurred while creating the championship", statusCode: 500);
        }
    }
}
