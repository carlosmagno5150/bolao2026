using BolaoApi.Domain.Entities;
using BolaoApi.Infrastructure.Repositories;
using BolaoApi.Tests.Fixtures;

namespace BolaoApi.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public UserRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    public void Dispose()
    {
        _fixture?.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var user = new User("John Doe", "john@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));

        // Act
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        // Assert
        var savedUser = await repository.GetByIdAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("John Doe", savedUser.Name);
        Assert.Equal("john@example.com", savedUser.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserWhenExists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var user = new User("Jane Doe", "jane@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        // Act
        var retrievedUser = await repository.GetByEmailAsync("jane@example.com");

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("Jane Doe", retrievedUser.Name);
        Assert.Equal("jane@example.com", retrievedUser.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNullWhenUserNotFound()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);

        // Act
        var retrievedUser = await repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserWhenExists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var user = new User("Bob Smith", "bob@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        // Act
        var retrievedUser = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("Bob Smith", retrievedUser.Name);
        Assert.Equal(user.Id, retrievedUser.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenUserNotFound()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrievedUser = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var user = new User("Alice Smith", "alice@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));

        // Act
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        var savedUser = await repository.GetByEmailAsync("alice@example.com");

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal("Alice Smith", savedUser.Name);
    }

    [Fact]
    public async Task AddAsync_MultipleUsers_ShouldAddAllUsers()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new UserRepository(context);
        var user1 = new User("User One", "user1@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));
        var user2 = new User("User Two", "user2@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));

        // Act
        await repository.AddAsync(user1);
        await repository.AddAsync(user2);
        await repository.SaveChangesAsync();

        // Assert
        var retrievedUser1 = await repository.GetByEmailAsync("user1@example.com");
        var retrievedUser2 = await repository.GetByEmailAsync("user2@example.com");

        Assert.NotNull(retrievedUser1);
        Assert.NotNull(retrievedUser2);
        Assert.Equal("User One", retrievedUser1.Name);
        Assert.Equal("User Two", retrievedUser2.Name);
    }
}
