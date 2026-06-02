using BolaoApi.Features.Teams.Domain.Entities;
using BolaoApi.Features.Teams.Infrastructure.Interfaces;
using BolaoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BolaoApi.Features.Teams.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams.ToListAsync();
    }

    public async Task<Team> CreateAsync(Team team)
    {
        team.Id = Guid.NewGuid();
        team.CreatedAt = DateTime.UtcNow;
        _context.Teams.Add(team);
        return team;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
