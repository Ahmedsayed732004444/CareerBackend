using Career_Path.Contracts.Common;
using Career_Path.Contracts.Roadmap;

namespace CareerPathFinal.Services;

public interface IRoadmapService
{
    Task<PaginatedList<RoadmapResponse>> GetUserRoadmapsAsync(
       string userId, RequestFilters filters, CancellationToken ct = default);

    Task<PaginatedList<RoadmapResponse>> GetUserSavedRoadmapsAsync(
       string userId, RequestFilters filters, CancellationToken ct = default);
    Task<Result<object>> GetRoadmapAsync(int id, CancellationToken ct = default);
    Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default);

}



