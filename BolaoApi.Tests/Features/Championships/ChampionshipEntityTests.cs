using BolaoApi.Features.Championships.Domain.Entities;

namespace BolaoApi.Tests.Features.Championships;

public class ChampionshipEntityTests
{
    [Fact]
    public void ChampionshipConstructor_ShouldHaveDefaultValues()
    {
        // Act
        var championship = new Championship();

        // Assert
        Assert.Equal(Guid.Empty, championship.Id);
        Assert.Equal(string.Empty, championship.Name);
        Assert.Equal(default(DateTime), championship.StartDate);
        Assert.Equal(default(DateTime), championship.EndDate);
        Assert.Equal(default(DateTime), championship.CreatedAt);
    }

    [Fact]
    public void ChampionshipProperties_ShouldBeSettable()
    {
        // Arrange
        var championship = new Championship();
        var newId = Guid.NewGuid();
        var newName = "Copa do Mundo 2026";
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var createdAt = DateTime.UtcNow.AddDays(-1);

        // Act
        championship.Id = newId;
        championship.Name = newName;
        championship.StartDate = startDate;
        championship.EndDate = endDate;
        championship.CreatedAt = createdAt;

        // Assert
        Assert.Equal(newId, championship.Id);
        Assert.Equal(newName, championship.Name);
        Assert.Equal(startDate, championship.StartDate);
        Assert.Equal(endDate, championship.EndDate);
        Assert.Equal(createdAt, championship.CreatedAt);
    }

    [Fact]
    public void Championship_NameCanBeAnyLength()
    {
        // Arrange
        var championship = new Championship();
        var longName = new string('A', 500);

        // Act
        championship.Name = longName;

        // Assert
        Assert.Equal(longName, championship.Name);
    }

    [Fact]
    public void Championship_StartAndEndDatesCanBeAnyValidDates()
    {
        // Arrange
        var championship = new Championship();
        var startDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        championship.StartDate = startDate;
        championship.EndDate = endDate;

        // Assert
        Assert.Equal(startDate, championship.StartDate);
        Assert.Equal(endDate, championship.EndDate);
    }

    [Fact]
    public void Championship_CreatedAtCanBeSetToAnyDate()
    {
        // Arrange
        var championship = new Championship();
        var pastDate = DateTime.UtcNow.AddYears(-10);

        // Act
        championship.CreatedAt = pastDate;

        // Assert
        Assert.Equal(pastDate, championship.CreatedAt);
    }

    [Fact]
    public void Championship_ShouldAllowSameStartAndEndDate()
    {
        // Arrange
        var championship = new Championship();
        var date = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);

        // Act
        championship.StartDate = date;
        championship.EndDate = date;

        // Assert
        Assert.Equal(date, championship.StartDate);
        Assert.Equal(date, championship.EndDate);
    }
}
