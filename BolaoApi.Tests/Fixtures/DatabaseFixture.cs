using BolaoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BolaoApi.Tests.Fixtures;

public class DatabaseFixture : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContext Context
    {
        get
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            return _context;
        }
    }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "BolaoApiTestDb_" + Guid.NewGuid())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
