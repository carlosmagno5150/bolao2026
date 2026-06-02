using BolaoApi.Domain.Entities;

namespace BolaoApi.Tests;

public class UserEntityTests
{
    [Fact]
    public void UserConstructor_ShouldInitializeUserProperties()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");

        // Act
        var user = new User(name, email, passwordHash);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.True(user.CreatedAt > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void UserConstructor_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var user1 = new User("User 1", "user1@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
        var user2 = new User("User 2", "user2@example.com", BCrypt.Net.BCrypt.HashPassword("password"));

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void UserConstructor_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var user = new User("Test", "test@example.com", BCrypt.Net.BCrypt.HashPassword("password"));

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(user.CreatedAt, beforeCreation, afterCreation);
    }

    [Fact]
    public void UserDefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var user = new User();

        // Assert
        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Name);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(default(DateTime), user.CreatedAt);
    }

    [Fact]
    public void UserProperties_ShouldBeSettable()
    {
        // Arrange
        var user = new User();
        var newId = Guid.NewGuid();
        var newName = "Updated Name";
        var newEmail = "updated@example.com";
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword("newpassword");
        var newCreatedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        user.Id = newId;
        user.Name = newName;
        user.Email = newEmail;
        user.PasswordHash = newPasswordHash;
        user.CreatedAt = newCreatedAt;

        // Assert
        Assert.Equal(newId, user.Id);
        Assert.Equal(newName, user.Name);
        Assert.Equal(newEmail, user.Email);
        Assert.Equal(newPasswordHash, user.PasswordHash);
        Assert.Equal(newCreatedAt, user.CreatedAt);
    }
}
