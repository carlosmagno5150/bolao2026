using BolaoApi.Features.Teams.Domain.Entities;
using BolaoApi.Features.Teams.Infrastructure.Repositories;
using BolaoApi.Tests.Fixtures;

namespace BolaoApi.Tests.Features.Teams;

public class TeamRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public TeamRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    public void Dispose()
    {
        _fixture?.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTeamWithUniqueId()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team = new Team { Name = "Test Team", UrlFlag = "https://example.com/flag.png" };

        // Act
        var createdTeam = await repository.CreateAsync(team);

        // Assert
        Assert.NotEqual(Guid.Empty, createdTeam.Id);
        Assert.Equal("Test Team", createdTeam.Name);
        Assert.Equal("https://example.com/flag.png", createdTeam.UrlFlag);
        Assert.NotEqual(default(DateTime), createdTeam.CreatedAt);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToNow()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team = new Team { Name = "Test Team", UrlFlag = "https://example.com/flag.png" };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var createdTeam = await repository.CreateAsync(team);

        // Assert
        Assert.InRange(createdTeam.CreatedAt, beforeCreation, DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveImmediately()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team = new Team { Name = "Test Team", UrlFlag = "https://example.com/flag.png" };

        // Act
        var createdTeam = await repository.CreateAsync(team);
        var foundTeam = await repository.GetByIdAsync(createdTeam.Id);

        // Assert
        Assert.Null(foundTeam); // Not saved yet
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTeamWhenExists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team = new Team { Name = "Brazil", UrlFlag = "https://example.com/brazil.png" };
        await repository.CreateAsync(team);
        await repository.SaveChangesAsync();

        // Act
        var retrievedTeam = await repository.GetByIdAsync(team.Id);

        // Assert
        Assert.NotNull(retrievedTeam);
        Assert.Equal("Brazil", retrievedTeam.Name);
        Assert.Equal("https://example.com/brazil.png", retrievedTeam.UrlFlag);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrievedTeam = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(retrievedTeam);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTeams()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team1 = new Team { Name = "Team 1", UrlFlag = "https://example.com/team1.png" };
        var team2 = new Team { Name = "Team 2", UrlFlag = "https://example.com/team2.png" };
        await repository.CreateAsync(team1);
        await repository.CreateAsync(team2);
        await repository.SaveChangesAsync();

        // Act
        var teams = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(teams);
        var teamList = teams.ToList();
        Assert.True(teamList.Count >= 2);
        Assert.Contains(teamList, t => t.Name == "Team 1");
        Assert.Contains(teamList, t => t.Name == "Team 2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyWhenNoTeams()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);

        // Act
        var teams = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(teams);
        Assert.Empty(teams);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team = new Team { Name = "Germany", UrlFlag = "https://example.com/germany.png" };
        await repository.CreateAsync(team);

        // Act
        await repository.SaveChangesAsync();
        var retrievedTeam = await repository.GetByIdAsync(team.Id);

        // Assert
        Assert.NotNull(retrievedTeam);
        Assert.Equal("Germany", retrievedTeam.Name);
    }

    [Fact]
    public async Task CreateAsync_MultipleTeams_ShouldHaveUniqueIds()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new TeamRepository(context);
        var team1 = new Team { Name = "Team A", UrlFlag = "https://example.com/a.png" };
        var team2 = new Team { Name = "Team B", UrlFlag = "https://example.com/b.png" };

        // Act
        var createdTeam1 = await repository.CreateAsync(team1);
        var createdTeam2 = await repository.CreateAsync(team2);

        // Assert
        Assert.NotEqual(createdTeam1.Id, createdTeam2.Id);
    }
}
