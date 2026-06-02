using BolaoApi.Features.Matches.Domain.Entities;
using BolaoApi.Features.Matches.Infrastructure.Repositories;
using BolaoApi.Tests.Fixtures;

namespace BolaoApi.Tests.Features.Matches;

public class MatchRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public MatchRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    public void Dispose()
    {
        _fixture?.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMatchWithUniqueId()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var match = new Match 
        { 
            ChampionshipId = championshipId,
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = homeTeamId,
            VisitorTeamId = visitorTeamId
        };

        // Act
        var createdMatch = await repository.CreateAsync(match);

        // Assert
        Assert.NotEqual(Guid.Empty, createdMatch.Id);
        Assert.Equal(championshipId, createdMatch.ChampionshipId);
        Assert.Equal(homeTeamId, createdMatch.HomeTeamId);
        Assert.Equal(visitorTeamId, createdMatch.VisitorTeamId);
        Assert.NotEqual(default(DateTime), createdMatch.CreatedAt);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToNow()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var match = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var createdMatch = await repository.CreateAsync(match);

        // Assert
        Assert.InRange(createdMatch.CreatedAt, beforeCreation, DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveImmediately()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var match = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };

        // Act
        var createdMatch = await repository.CreateAsync(match);
        var foundMatch = await repository.GetByIdAsync(createdMatch.Id);

        // Assert
        Assert.Null(foundMatch); // Not saved yet
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMatchWhenExists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var championshipId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var visitorTeamId = Guid.NewGuid();
        var matchDate = new DateTime(2026, 6, 21, 16, 0, 0, DateTimeKind.Utc);
        var match = new Match 
        { 
            ChampionshipId = championshipId,
            Date = matchDate,
            HomeTeamId = homeTeamId,
            VisitorTeamId = visitorTeamId,
            ScoreHomeTeam = 2,
            ScoreVisitorTeam = 1
        };
        await repository.CreateAsync(match);
        await repository.SaveChangesAsync();

        // Act
        var retrievedMatch = await repository.GetByIdAsync(match.Id);

        // Assert
        Assert.NotNull(retrievedMatch);
        Assert.Equal(championshipId, retrievedMatch.ChampionshipId);
        Assert.Equal(homeTeamId, retrievedMatch.HomeTeamId);
        Assert.Equal(visitorTeamId, retrievedMatch.VisitorTeamId);
        Assert.Equal(2, retrievedMatch.ScoreHomeTeam);
        Assert.Equal(1, retrievedMatch.ScoreVisitorTeam);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrievedMatch = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(retrievedMatch);
    }

    [Fact]
    public async Task GetByChampionshipIdAsync_ShouldReturnMatchesForChampionship()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var championshipId = Guid.NewGuid();
        var match1 = new Match 
        { 
            ChampionshipId = championshipId,
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        var match2 = new Match 
        { 
            ChampionshipId = championshipId,
            Date = DateTime.UtcNow.AddDays(2),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        await repository.CreateAsync(match1);
        await repository.CreateAsync(match2);
        await repository.SaveChangesAsync();

        // Act
        var matches = await repository.GetByChampionshipIdAsync(championshipId);

        // Assert
        Assert.NotNull(matches);
        var matchList = matches.ToList();
        Assert.Equal(2, matchList.Count);
        Assert.All(matchList, m => Assert.Equal(championshipId, m.ChampionshipId));
    }

    [Fact]
    public async Task GetByChampionshipIdAsync_ShouldReturnEmptyWhenNoMatches()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var championshipId = Guid.NewGuid();

        // Act
        var matches = await repository.GetByChampionshipIdAsync(championshipId);

        // Assert
        Assert.NotNull(matches);
        Assert.Empty(matches);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMatches()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var match1 = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        var match2 = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(2),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        await repository.CreateAsync(match1);
        await repository.CreateAsync(match2);
        await repository.SaveChangesAsync();

        // Act
        var matches = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(matches);
        var matchList = matches.ToList();
        Assert.True(matchList.Count >= 2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyWhenNoMatches()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);

        // Act
        var matches = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(matches);
        Assert.Empty(matches);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var match = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        await repository.CreateAsync(match);

        // Act
        await repository.SaveChangesAsync();
        var retrievedMatch = await repository.GetByIdAsync(match.Id);

        // Assert
        Assert.NotNull(retrievedMatch);
    }

    [Fact]
    public async Task CreateAsync_MultipleMatches_ShouldHaveUniqueIds()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var match1 = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        var match2 = new Match 
        { 
            ChampionshipId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(2),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };

        // Act
        var createdMatch1 = await repository.CreateAsync(match1);
        var createdMatch2 = await repository.CreateAsync(match2);

        // Assert
        Assert.NotEqual(createdMatch1.Id, createdMatch2.Id);
    }

    [Fact]
    public async Task GetByChampionshipIdAsync_ShouldNotReturnMatchesFromOtherChampionships()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new MatchRepository(context);
        var championshipId1 = Guid.NewGuid();
        var championshipId2 = Guid.NewGuid();
        var match1 = new Match 
        { 
            ChampionshipId = championshipId1,
            Date = DateTime.UtcNow.AddDays(1),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        var match2 = new Match 
        { 
            ChampionshipId = championshipId2,
            Date = DateTime.UtcNow.AddDays(2),
            HomeTeamId = Guid.NewGuid(),
            VisitorTeamId = Guid.NewGuid()
        };
        await repository.CreateAsync(match1);
        await repository.CreateAsync(match2);
        await repository.SaveChangesAsync();

        // Act
        var matches = await repository.GetByChampionshipIdAsync(championshipId1);

        // Assert
        Assert.Single(matches);
        Assert.Equal(championshipId1, matches.First().ChampionshipId);
    }
}
