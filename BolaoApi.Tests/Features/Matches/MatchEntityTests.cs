using BolaoApi.Features.Matches.Domain.Entities;

namespace BolaoApi.Tests.Features.Matches;

public class MatchEntityTests
{
    [Fact]
    public void MatchConstructor_ShouldHaveDefaultValues()
    {
        // Act
        var match = new Match();

        // Assert
        Assert.Equal(Guid.Empty, match.Id);
        Assert.Equal(Guid.Empty, match.ChampionshipId);
        Assert.Equal(default(DateTime), match.Date);
        Assert.Equal(Guid.Empty, match.HomeTeamId);
        Assert.Equal(Guid.Empty, match.VisitorTeamId);
        Assert.Null(match.ScoreHomeTeam);
        Assert.Null(match.ScoreVisitorTeam);
        Assert.Equal(default(DateTime), match.CreatedAt);
    }

    [Fact]
    public void MatchProperties_ShouldBeSettable()
    {
        // Arrange
        var match = new Match();
        var newId = Guid.NewGuid();
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var date = new DateTime(2026, 6, 21, 16, 0, 0, DateTimeKind.Utc);
        var createdAt = DateTime.UtcNow.AddDays(-1);

        // Act
        match.Id = newId;
        match.ChampionshipId = championshipId;
        match.Date = date;
        match.HomeTeamId = homeTeamId;
        match.VisitorTeamId = visitorTeamId;
        match.ScoreHomeTeam = 2;
        match.ScoreVisitorTeam = 1;
        match.CreatedAt = createdAt;

        // Assert
        Assert.Equal(newId, match.Id);
        Assert.Equal(championshipId, match.ChampionshipId);
        Assert.Equal(date, match.Date);
        Assert.Equal(homeTeamId, match.HomeTeamId);
        Assert.Equal(visitorTeamId, match.VisitorTeamId);
        Assert.Equal(2, match.ScoreHomeTeam);
        Assert.Equal(1, match.ScoreVisitorTeam);
        Assert.Equal(createdAt, match.CreatedAt);
    }

    [Fact]
    public void Match_ScoresCanBeNull()
    {
        // Arrange
        var match = new Match { ScoreHomeTeam = 2, ScoreVisitorTeam = 1 };

        // Act
        match.ScoreHomeTeam = null;
        match.ScoreVisitorTeam = null;

        // Assert
        Assert.Null(match.ScoreHomeTeam);
        Assert.Null(match.ScoreVisitorTeam);
    }

    [Fact]
    public void Match_ScoresCanBeZero()
    {
        // Arrange
        var match = new Match();

        // Act
        match.ScoreHomeTeam = 0;
        match.ScoreVisitorTeam = 0;

        // Assert
        Assert.Equal(0, match.ScoreHomeTeam);
        Assert.Equal(0, match.ScoreVisitorTeam);
    }

    [Fact]
    public void Match_ScoresCanBeHighValues()
    {
        // Arrange
        var match = new Match();

        // Act
        match.ScoreHomeTeam = 10;
        match.ScoreVisitorTeam = 9;

        // Assert
        Assert.Equal(10, match.ScoreHomeTeam);
        Assert.Equal(9, match.ScoreVisitorTeam);
    }

    [Fact]
    public void Match_DateCanBeAnyValidDateTime()
    {
        // Arrange
        var match = new Match();
        var futureDate = DateTime.UtcNow.AddYears(1);

        // Act
        match.Date = futureDate;

        // Assert
        Assert.Equal(futureDate, match.Date);
    }

    [Fact]
    public void Match_CanHaveSameHomeAndVisitorTeamIds()
    {
        // Arrange
        var match = new Match();
        var teamId = Guid.NewGuid();

        // Act
        match.HomeTeamId = teamId;
        match.VisitorTeamId = teamId;

        // Assert
        Assert.Equal(teamId, match.HomeTeamId);
        Assert.Equal(teamId, match.VisitorTeamId);
    }

    [Fact]
    public void Match_OneScoreCanBeNullWhileOtherIsSet()
    {
        // Arrange
        var match = new Match();

        // Act
        match.ScoreHomeTeam = 2;
        match.ScoreVisitorTeam = null;

        // Assert
        Assert.Equal(2, match.ScoreHomeTeam);
        Assert.Null(match.ScoreVisitorTeam);
    }
}
