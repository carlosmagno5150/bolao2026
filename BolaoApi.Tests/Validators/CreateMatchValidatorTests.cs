using BolaoApi.Features.Matches.Application.Dtos;
using BolaoApi.Features.Matches.Application.Validations;

namespace BolaoApi.Tests.Validators;

public class CreateMatchValidatorTests
{
    [Fact]
    public async Task ValidateAsync_WithValidData_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithValidDataAndScores_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            2,
            1
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyChampionshipId_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            Guid.Empty,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ChampionshipId");
    }

    [Fact]
    public async Task ValidateAsync_WithMinDateAsDate_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.MinValue,
            homeTeamId,
            visitorTeamId,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Date");
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyHomeTeamId_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            Guid.Empty,
            visitorTeamId,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "HomeTeamId");
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyVisitorTeamId_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            Guid.Empty,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VisitorTeamId");
    }

    [Fact]
    public async Task ValidateAsync_WithSameHomeAndVisitorTeamIds_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            teamId,
            teamId,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VisitorTeamId" && e.ErrorMessage.Contains("different"));
    }

    [Fact]
    public async Task ValidateAsync_WithNegativeHomeScore_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            -1,
            1
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ScoreHomeTeam");
    }

    [Fact]
    public async Task ValidateAsync_WithNegativeVisitorScore_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            1,
            -1
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ScoreVisitorTeam");
    }

    [Fact]
    public async Task ValidateAsync_WithZeroScores_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            0,
            0
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithHighScores_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            10,
            9
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyHomeScore_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            2,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithOnlyVisitorScore_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var request = new CreateMatchRequestDto(
            championshipId,
            DateTime.UtcNow.AddDays(1),
            homeTeamId,
            visitorTeamId,
            null,
            1
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithAllInvalidFields_ShouldReturnMultipleErrors()
    {
        // Arrange
        var validator = new CreateMatchValidator();
        var request = new CreateMatchRequestDto(
            Guid.Empty,
            DateTime.MinValue,
            Guid.Empty,
            Guid.Empty,
            null,
            null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}
