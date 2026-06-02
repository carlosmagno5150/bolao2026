using BolaoApi.Features.Championships.Domain.Entities;
using BolaoApi.Features.Championships.Infrastructure.Interfaces;
using BolaoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BolaoApi.Features.Championships.Infrastructure.Repositories;

public class ChampionshipRepository : IChampionshipRepository
{
    private readonly ApplicationDbContext _context;

    public ChampionshipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Championship?> GetByIdAsync(Guid id)
    {
        return await _context.Championships.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Championship>> GetAllAsync()
    {
        return await _context.Championships.ToListAsync();
    }

    public async Task<Championship> CreateAsync(Championship championship)
    {
        championship.Id = Guid.NewGuid();
        championship.CreatedAt = DateTime.UtcNow;
        _context.Championships.Add(championship);
        return championship;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
