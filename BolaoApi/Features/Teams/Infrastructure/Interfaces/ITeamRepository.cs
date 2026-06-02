using BolaoApi.Features.Teams.Domain.Entities;

namespace BolaoApi.Features.Teams.Infrastructure.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team> CreateAsync(Team team);
    Task SaveChangesAsync();
}
