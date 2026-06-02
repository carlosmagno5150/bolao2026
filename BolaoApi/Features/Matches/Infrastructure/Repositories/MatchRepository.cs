using BolaoApi.Features.Matches.Domain.Entities;
using BolaoApi.Features.Matches.Infrastructure.Interfaces;
using BolaoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BolaoApi.Features.Matches.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly ApplicationDbContext _context;

    public MatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetByChampionshipIdAsync(Guid championshipId)
    {
        return await _context.Matches.Where(m => m.ChampionshipId == championshipId).ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        return await _context.Matches.ToListAsync();
    }

    public async Task<Match> CreateAsync(Match match)
    {
        match.Id = Guid.NewGuid();
        match.CreatedAt = DateTime.UtcNow;
        _context.Matches.Add(match);
        return match;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
