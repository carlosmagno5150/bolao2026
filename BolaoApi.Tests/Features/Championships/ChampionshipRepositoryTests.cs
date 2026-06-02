using BolaoApi.Features.Championships.Domain.Entities;
using BolaoApi.Features.Championships.Infrastructure.Repositories;
using BolaoApi.Tests.Fixtures;

namespace BolaoApi.Tests.Features.Championships;

public class ChampionshipRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public ChampionshipRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    public void Dispose()
    {
        _fixture?.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateChampionshipWithUniqueId()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var championship = new Championship 
        { 
            Name = "Copa do Mundo 2026", 
            StartDate = startDate, 
            EndDate = endDate 
        };

        // Act
        var createdChampionship = await repository.CreateAsync(championship);

        // Assert
        Assert.NotEqual(Guid.Empty, createdChampionship.Id);
        Assert.Equal("Copa do Mundo 2026", createdChampionship.Name);
        Assert.Equal(startDate, createdChampionship.StartDate);
        Assert.Equal(endDate, createdChampionship.EndDate);
        Assert.NotEqual(default(DateTime), createdChampionship.CreatedAt);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToNow()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var championship = new Championship 
        { 
            Name = "Test Championship", 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var createdChampionship = await repository.CreateAsync(championship);

        // Assert
        Assert.InRange(createdChampionship.CreatedAt, beforeCreation, DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveImmediately()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var championship = new Championship 
        { 
            Name = "Test Championship", 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var createdChampionship = await repository.CreateAsync(championship);
        var foundChampionship = await repository.GetByIdAsync(createdChampionship.Id);

        // Assert
        Assert.Null(foundChampionship); // Not saved yet
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnChampionshipWhenExists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var championship = new Championship 
        { 
            Name = "World Cup 2026", 
            StartDate = startDate,
            EndDate = endDate
        };
        await repository.CreateAsync(championship);
        await repository.SaveChangesAsync();

        // Act
        var retrievedChampionship = await repository.GetByIdAsync(championship.Id);

        // Assert
        Assert.NotNull(retrievedChampionship);
        Assert.Equal("World Cup 2026", retrievedChampionship.Name);
        Assert.Equal(startDate, retrievedChampionship.StartDate);
        Assert.Equal(endDate, retrievedChampionship.EndDate);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrievedChampionship = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(retrievedChampionship);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllChampionships()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var championship1 = new Championship 
        { 
            Name = "Championship 1", 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };
        var championship2 = new Championship 
        { 
            Name = "Championship 2", 
            StartDate = DateTime.UtcNow.AddDays(20),
            EndDate = DateTime.UtcNow.AddDays(30)
        };
        await repository.CreateAsync(championship1);
        await repository.CreateAsync(championship2);
        await repository.SaveChangesAsync();

        // Act
        var championships = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(championships);
        var championshipList = championships.ToList();
        Assert.True(championshipList.Count >= 2);
        Assert.Contains(championshipList, c => c.Name == "Championship 1");
        Assert.Contains(championshipList, c => c.Name == "Championship 2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyWhenNoChampionships()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);

        // Act
        var championships = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(championships);
        Assert.Empty(championships);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var startDate = new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 7, 18, 0, 0, 0, DateTimeKind.Utc);
        var championship = new Championship 
        { 
            Name = "Euro 2024", 
            StartDate = startDate,
            EndDate = endDate
        };
        await repository.CreateAsync(championship);

        // Act
        await repository.SaveChangesAsync();
        var retrievedChampionship = await repository.GetByIdAsync(championship.Id);

        // Assert
        Assert.NotNull(retrievedChampionship);
        Assert.Equal("Euro 2024", retrievedChampionship.Name);
    }

    [Fact]
    public async Task CreateAsync_MultipleChampionships_ShouldHaveUniqueIds()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new ChampionshipRepository(context);
        var championship1 = new Championship 
        { 
            Name = "Champ A", 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };
        var championship2 = new Championship 
        { 
            Name = "Champ B", 
            StartDate = DateTime.UtcNow.AddDays(20),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var createdChamp1 = await repository.CreateAsync(championship1);
        var createdChamp2 = await repository.CreateAsync(championship2);

        // Assert
        Assert.NotEqual(createdChamp1.Id, createdChamp2.Id);
    }
}
