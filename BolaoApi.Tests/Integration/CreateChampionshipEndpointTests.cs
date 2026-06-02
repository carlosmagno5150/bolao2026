using BolaoApi.Features.Championships.Application.Dtos;
using BolaoApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BolaoApi.Tests.Integration;

public class CreateChampionshipEndpointTests : IAsyncLifetime
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
    public async Task CreateChampionshipEndpoint_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "Copa do Mundo 2026",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateChampionshipResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Copa do Mundo 2026", result.Name);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithShortName_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "AB",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithEndDateBeforeStartDate_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "Championship",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithSameDateAsBothDates_ShouldReturnCreated()
    {
        // Arrange
        var date = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "Single Day Championship",
            StartDate: date,
            EndDate: date
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithValidEndDateAfterStartDate_ShouldReturnCreated()
    {
        // Arrange
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "Long Championship",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_ShouldReturnLocationHeader()
    {
        // Arrange
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: "World Cup",
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.NotNull(response.Headers.Location);
        Assert.True(response.Headers.Location.ToString().StartsWith("/championships/"));
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithMaxNameLength_ShouldReturnCreated()
    {
        // Arrange
        var maxName = new string('A', 255);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: maxName,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_WithExceededNameLength_ShouldReturnBadRequest()
    {
        // Arrange
        var tooLongName = new string('A', 256);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(
            Name: tooLongName,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var response = await _client.PostAsJsonAsync("/championships", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampionshipEndpoint_MultipleChampionships_ShouldCreateAll()
    {
        // Arrange
        var champ1 = new CreateChampionshipRequestDto(
            Name: "Championship 1",
            StartDate: new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc),
            EndDate: new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc)
        );
        var champ2 = new CreateChampionshipRequestDto(
            Name: "Championship 2",
            StartDate: new DateTime(2027, 6, 21, 0, 0, 0, DateTimeKind.Utc),
            EndDate: new DateTime(2027, 7, 18, 0, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var response1 = await _client.PostAsJsonAsync("/championships", champ1);
        var response2 = await _client.PostAsJsonAsync("/championships", champ2);

        // Assert
        Assert.True(response1.IsSuccessStatusCode);
        Assert.True(response2.IsSuccessStatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateChampionshipResponseDto>();
        var result2 = await response2.Content.ReadFromJsonAsync<CreateChampionshipResponseDto>();
        Assert.NotEqual(result1?.Id, result2?.Id);
    }
}
