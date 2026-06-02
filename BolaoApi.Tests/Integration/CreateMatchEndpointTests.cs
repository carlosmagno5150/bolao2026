using BolaoApi.Features.Matches.Application.Dtos;
using BolaoApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BolaoApi.Tests.Integration;

public class CreateMatchEndpointTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("environment", "Testing");
            });

        _client = _factory.CreateClient();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();

        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var matchDate = new DateTime(2026, 6, 21, 16, 0, 0, DateTimeKind.Utc);
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: matchDate,
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateMatchResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(championshipId, result.ChampionshipId);
        Assert.Equal(homeTeamId, result.HomeTeamId);
        Assert.Equal(visitorTeamId, result.VisitorTeamId);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithValidDataAndScores_ShouldReturnCreated()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var matchDate = new DateTime(2026, 6, 21, 16, 0, 0, DateTimeKind.Utc);
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: matchDate,
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: 2,
            ScoreVisitorTeam: 1
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateMatchResponseDto>();
        Assert.NotNull(result);
        Assert.Equal(2, result.ScoreHomeTeam);
        Assert.Equal(1, result.ScoreVisitorTeam);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithEmptyChampionshipId_ShouldReturnBadRequest()
    {
        // Arrange
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: Guid.Empty,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithEmptyHomeTeamId_ShouldReturnBadRequest()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: Guid.Empty,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithEmptyVisitorTeamId_ShouldReturnBadRequest()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: Guid.Empty,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithSameTeamIds_ShouldReturnBadRequest()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: teamId,
            VisitorTeamId: teamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithNegativeHomeScore_ShouldReturnBadRequest()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: -1,
            ScoreVisitorTeam: 1
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithNegativeVisitorScore_ShouldReturnBadRequest()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: 2,
            ScoreVisitorTeam: -1
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithZeroScores_ShouldReturnCreated()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: 0,
            ScoreVisitorTeam: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_ShouldReturnLocationHeader()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.NotNull(response.Headers.Location);
        Assert.True(response.Headers.Location.ToString().StartsWith("/matches/"));
    }

    [Fact]
    public async Task CreateMatchEndpoint_MultipleMatches_ShouldCreateAll()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var match1 = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: Guid.NewGuid(),
            VisitorTeamId: Guid.NewGuid(),
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );
        var match2 = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(2),
            HomeTeamId: Guid.NewGuid(),
            VisitorTeamId: Guid.NewGuid(),
            ScoreHomeTeam: null,
            ScoreVisitorTeam: null
        );

        // Act
        var response1 = await _client.PostAsJsonAsync("/matches", match1);
        var response2 = await _client.PostAsJsonAsync("/matches", match2);

        // Assert
        Assert.True(response1.IsSuccessStatusCode);
        Assert.True(response2.IsSuccessStatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateMatchResponseDto>();
        var result2 = await response2.Content.ReadFromJsonAsync<CreateMatchResponseDto>();
        Assert.NotEqual(result1?.Id, result2?.Id);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithOnlyHomeScore_ShouldReturnCreated()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: 2,
            ScoreVisitorTeam: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatchEndpoint_WithOnlyVisitorScore_ShouldReturnCreated()
    {
        // Arrange
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            ChampionshipId: championshipId,
            Date: DateTime.UtcNow.AddDays(1),
            HomeTeamId: homeTeamId,
            VisitorTeamId: visitorTeamId,
            ScoreHomeTeam: null,
            ScoreVisitorTeam: 1
        );

        // Act
        var response = await _client.PostAsJsonAsync("/matches", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
