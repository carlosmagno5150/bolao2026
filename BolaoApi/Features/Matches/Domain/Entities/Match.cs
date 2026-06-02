namespace BolaoApi.Features.Matches.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }
    public Guid ChampionshipId { get; set; }
    public DateTime Date { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid VisitorTeamId { get; set; }
    public int? ScoreHomeTeam { get; set; }
    public int? ScoreVisitorTeam { get; set; }
    public DateTime CreatedAt { get; set; }
}
