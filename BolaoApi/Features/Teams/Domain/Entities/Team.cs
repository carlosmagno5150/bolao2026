namespace BolaoApi.Features.Teams.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UrlFlag { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
