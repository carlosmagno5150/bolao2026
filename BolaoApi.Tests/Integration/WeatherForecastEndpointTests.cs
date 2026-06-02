using Microsoft.AspNetCore.Mvc.Testing;
using BolaoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BolaoApi.Tests.Integration;

public class WeatherForecastEndpointTests : IAsyncLifetime
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
    public async Task GetWeatherForecast_ShouldReturnOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnArrayOfForecasts()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadFromJsonAsync<dynamic[]>();

        // Assert
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        Assert.Equal(5, content.Length);
    }

    [Fact]
    public async Task GetWeatherForecast_ShouldReturnForecasWithValidData()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>[]>();

        // Assert
        Assert.NotNull(content);
        foreach (var forecast in content)
        {
            Assert.True(forecast.ContainsKey("date"));
            Assert.True(forecast.ContainsKey("temperatureC"));
            Assert.True(forecast.ContainsKey("summary"));
            Assert.True(forecast.ContainsKey("temperatureF"));
        }
    }
}
