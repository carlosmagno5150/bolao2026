using BolaoApi.Features.Championships.Application.Dtos;
using BolaoApi.Features.Championships.Application.Validations;

namespace BolaoApi.Tests.Validators;

public class CreateChampionshipValidatorTests
{
    [Fact]
    public async Task ValidateAsync_WithValidData_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Copa do Mundo 2026", startDate, endDate);

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
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("", startDate, endDate);

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
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("AB", startDate, endDate);

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
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("ABC", startDate, endDate);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithNameTooLong_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var longName = new string('A', 256);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(longName, startDate, endDate);

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
        var validator = new CreateChampionshipValidator();
        var name = new string('A', 255);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto(name, startDate, endDate);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithEndDateBeforeStartDate_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Championship", startDate, endDate);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate" && e.ErrorMessage.Contains("after start date"));
    }

    [Fact]
    public async Task ValidateAsync_WithSameStartAndEndDate_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var date = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Championship", date, date);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithEndDateAfterStartDate_ShouldReturnValid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Championship", startDate, endDate);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithMinDateAsStartDate_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var endDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Championship", DateTime.MinValue, endDate);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StartDate");
    }

    [Fact]
    public async Task ValidateAsync_WithMinDateAsEndDate_ShouldReturnInvalid()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateChampionshipRequestDto("Championship", startDate, DateTime.MinValue);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task ValidateAsync_WithAllInvalidFields_ShouldReturnMultipleErrors()
    {
        // Arrange
        var validator = new CreateChampionshipValidator();
        var request = new CreateChampionshipRequestDto("", DateTime.MinValue, DateTime.MinValue);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }
}
