using BolaoApi.Features.Matches.Domain.Entities;

namespace BolaoApi.Features.Matches.Infrastructure.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByChampionshipIdAsync(Guid championshipId);
    Task<IEnumerable<Match>> GetAllAsync();
    Task<Match> CreateAsync(Match match);
    Task SaveChangesAsync();
}
