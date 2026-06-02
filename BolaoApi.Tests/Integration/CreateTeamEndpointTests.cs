using BolaoApi.Features.Teams.Application.Dtos;
using BolaoApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BolaoApi.Tests.Integration;

public class CreateTeamEndpointTests : IAsyncLifetime
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
    public async Task CreateTeamEndpoint_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "Brazil",
            UrlFlag: "https://example.com/brazil.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateTeamResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Brazil", result.Name);
        Assert.Equal("https://example.com/brazil.png", result.UrlFlag);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "",
            UrlFlag: "https://example.com/flag.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithShortName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "BR",
            UrlFlag: "https://example.com/brazil.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithInvalidUrl_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "Brazil",
            UrlFlag: "not-a-url"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithEmptyUrl_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "Brazil",
            UrlFlag: ""
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithValidHttpUrl_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "Germany",
            UrlFlag: "http://example.com/germany.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_ShouldReturnLocationHeader()
    {
        // Arrange
        var request = new CreateTeamRequestDto(
            Name: "Argentina",
            UrlFlag: "https://example.com/argentina.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.NotNull(response.Headers.Location);
        Assert.True(response.Headers.Location.ToString().StartsWith("/teams/"));
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithMaxNameLength_ShouldReturnCreated()
    {
        // Arrange
        var maxName = new string('A', 255);
        var request = new CreateTeamRequestDto(
            Name: maxName,
            UrlFlag: "https://example.com/flag.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_WithExceededNameLength_ShouldReturnBadRequest()
    {
        // Arrange
        var tooLongName = new string('A', 256);
        var request = new CreateTeamRequestDto(
            Name: tooLongName,
            UrlFlag: "https://example.com/flag.png"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/teams", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTeamEndpoint_MultipleTeams_ShouldCreateAll()
    {
        // Arrange
        var team1 = new CreateTeamRequestDto("Team A", "https://example.com/a.png");
        var team2 = new CreateTeamRequestDto("Team B", "https://example.com/b.png");

        // Act
        var response1 = await _client.PostAsJsonAsync("/teams", team1);
        var response2 = await _client.PostAsJsonAsync("/teams", team2);

        // Assert
        Assert.True(response1.IsSuccessStatusCode);
        Assert.True(response2.IsSuccessStatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateTeamResponseDto>();
        var result2 = await response2.Content.ReadFromJsonAsync<CreateTeamResponseDto>();
        Assert.NotEqual(result1?.Id, result2?.Id);
    }
}
