using Career_Path.Contracts.Match;

namespace CareerPathFinal.Services
{
    public interface IMatchService
    {
        Task<Result<List<JobMatchResult>>> GetMatchAsync(string userId, CancellationToken cancellationToken);
    }
}
