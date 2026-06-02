using BolaoApi.Features.Teams.Domain.Entities;

namespace BolaoApi.Tests.Features.Teams;

public class TeamEntityTests
{
    [Fact]
    public void TeamConstructor_ShouldHaveDefaultValues()
    {
        // Act
        var team = new Team();

        // Assert
        Assert.Equal(Guid.Empty, team.Id);
        Assert.Equal(string.Empty, team.Name);
        Assert.Equal(string.Empty, team.UrlFlag);
        Assert.Equal(default(DateTime), team.CreatedAt);
    }

    [Fact]
    public void TeamProperties_ShouldBeSettable()
    {
        // Arrange
        var team = new Team();
        var newId = Guid.NewGuid();
        var newName = "Updated Team";
        var newUrlFlag = "https://example.com/updated-flag.png";
        var newCreatedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        team.Id = newId;
        team.Name = newName;
        team.UrlFlag = newUrlFlag;
        team.CreatedAt = newCreatedAt;

        // Assert
        Assert.Equal(newId, team.Id);
        Assert.Equal(newName, team.Name);
        Assert.Equal(newUrlFlag, team.UrlFlag);
        Assert.Equal(newCreatedAt, team.CreatedAt);
    }

    [Fact]
    public void Team_NameCanBeAnyLength()
    {
        // Arrange
        var team = new Team();
        var longName = new string('A', 500);

        // Act
        team.Name = longName;

        // Assert
        Assert.Equal(longName, team.Name);
    }

    [Fact]
    public void Team_UrlFlagCanBeAnyValidUrl()
    {
        // Arrange
        var team = new Team();
        var url = "https://example.com/flags/brazil.png";

        // Act
        team.UrlFlag = url;

        // Assert
        Assert.Equal(url, team.UrlFlag);
    }

    [Fact]
    public void Team_CreatedAtCanBeSetToAnyDate()
    {
        // Arrange
        var team = new Team();
        var pastDate = DateTime.UtcNow.AddYears(-5);

        // Act
        team.CreatedAt = pastDate;

        // Assert
        Assert.Equal(pastDate, team.CreatedAt);
    }
}
