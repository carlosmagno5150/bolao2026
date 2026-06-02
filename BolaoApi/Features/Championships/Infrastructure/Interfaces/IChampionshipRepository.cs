using BolaoApi.Features.Championships.Domain.Entities;

namespace BolaoApi.Features.Championships.Infrastructure.Interfaces;

public interface IChampionshipRepository
{
    Task<Championship?> GetByIdAsync(Guid id);
    Task<IEnumerable<Championship>> GetAllAsync();
    Task<Championship> CreateAsync(Championship championship);
    Task SaveChangesAsync();
}
