using BolaoApi.Application.Dtos;
using BolaoApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace BolaoApi.Tests.Integration;

public class AuthEndpointTests : IAsyncLifetime
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
    public async Task RegisterEndpoint_WithValidData_ShouldReturnOkAndUserId()
    {
        // Arrange
        var request = new RegisterRequestDto(
            Name: "Test User",
            Email: "test@example.com",
            Password: "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Contains("successfully", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterEndpoint_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto(
            Name: "Test User",
            Email: "duplicate@example.com",
            Password: "Password123!"
        );

        // Register first user
        await _client.PostAsJsonAsync("/auth/register", request);

        // Act - Try to register with same email
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateEndpoint_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto(
            Name: "Auth Test User",
            Email: "auth@example.com",
            Password: "Password123!"
        );

        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        var authRequest = new AuthenticateRequestDto(
            Email: "auth@example.com",
            Password: "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/auth/authenticate", authRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthenticateResponseDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task AuthenticateEndpoint_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        var authRequest = new AuthenticateRequestDto(
            Email: "nonexistent@example.com",
            Password: "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/auth/authenticate", authRequest);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateEndpoint_WithWrongPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto(
            Name: "Wrong Pass User",
            Email: "wrongpass@example.com",
            Password: "CorrectPassword123!"
        );

        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        var authRequest = new AuthenticateRequestDto(
            Email: "wrongpass@example.com",
            Password: "WrongPassword123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/auth/authenticate", authRequest);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateEndpoint_TokenShouldBeValid()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto(
            Name: "Token Test User",
            Email: "tokentest@example.com",
            Password: "Password123!"
        );

        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        var authRequest = new AuthenticateRequestDto(
            Email: "tokentest@example.com",
            Password: "Password123!"
        );

        var authResponse = await _client.PostAsJsonAsync("/auth/authenticate", authRequest);
        var authResult = await authResponse.Content.ReadFromJsonAsync<AuthenticateResponseDto>();

        // Act - Verify token is not empty and has proper JWT format
        var tokenParts = authResult?.Token?.Split('.') ?? Array.Empty<string>();

        // Assert - JWT should have 3 parts separated by dots
        Assert.Equal(3, tokenParts.Length);
        Assert.NotEmpty(tokenParts[0]); // Header
        Assert.NotEmpty(tokenParts[1]); // Payload
        Assert.NotEmpty(tokenParts[2]); // Signature
    }
}
