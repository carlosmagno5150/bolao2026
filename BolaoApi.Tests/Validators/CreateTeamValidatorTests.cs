using BolaoApi.Features.Teams.Application.Dtos;
using BolaoApi.Features.Teams.Application.Validations;

namespace BolaoApi.Tests.Validators;

public class CreateTeamValidatorTests
{
    [Fact]
    public async Task ValidateAsync_WithValidData_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyName_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("", "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task ValidateAsync_WithNameTooShort_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("AB", "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("at least 3"));
    }

    [Fact]
    public async Task ValidateAsync_WithNameExactly3Chars_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("ABC", "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithNameTooLong_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var longName = new string('A', 256);
        var request = new CreateTeamRequestDto(longName, "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("not exceed 255"));
    }

    [Fact]
    public async Task ValidateAsync_WithNameExactly255Chars_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var name = new string('A', 255);
        var request = new CreateTeamRequestDto(name, "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyUrlFlag_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UrlFlag");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidUrl_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "not a url");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UrlFlag" && e.ErrorMessage.Contains("valid URL"));
    }

    [Fact]
    public async Task ValidateAsync_WithHttpUrl_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "http://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithHttpsUrl_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "https://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithFtpUrl_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("Team Name", "ftp://example.com/flag.png");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UrlFlag");
    }

    [Fact]
    public async Task ValidateAsync_WithAllInvalidFields_ShouldReturnMultipleErrors()
    {
        // Arrange
        var validator = new CreateTeamValidator();
        var request = new CreateTeamRequestDto("", "");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }
}
