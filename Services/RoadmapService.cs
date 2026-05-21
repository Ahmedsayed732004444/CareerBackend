using Career_Path.Contracts.Common;
using Career_Path.Contracts.Roadmap;
using CareerPathFinal.Services;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Career_Path.Services;

public class RoadmapService(
    ApplicationDbContext context,
    ILogger<RoadmapService> logger) : IRoadmapService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<RoadmapService> _logger = logger;

    public async Task<PaginatedList<RoadmapResponse>> GetUserRoadmapsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.RoadmapJsons
                .Where(r => r.ApplicationUserId == userId)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.RoadmapData.Contains(filters.SearchValue!))
                .Select(r => new RoadmapResponse(r.Id, r.CreatedAt, r.IsSaved))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving roadmaps for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedList<RoadmapResponse>> GetUserSavedRoadmapsAsync(
        string userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.RoadmapJsons
                .Where(r => r.ApplicationUserId == userId && r.IsSaved == true)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.RoadmapData.Contains(filters.SearchValue!))
                .Select(r => new RoadmapResponse(r.Id, r.CreatedAt, r.IsSaved))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving saved roadmaps for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Result<object>> GetRoadmapAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var roadmap = await _context.RoadmapJsons
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (roadmap is null)
                return Result.Failure<object>(RoadmapErrors.NotFound);

            var jsonData = JsonDocument.Parse(roadmap.RoadmapData).RootElement;

            return Result.Success<object>(new
            {
                roadmap.Id,
                RoadmapData = jsonData,
                roadmap.CreatedAt,
                roadmap.IsSaved
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving roadmap {RoadmapId}", id);
            return Result.Failure<object>(RoadmapErrors.Error);
        }
    }

    public async Task<Result> ToggleStatusAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var roadmap = await _context.RoadmapJsons
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (roadmap is null)
                return Result.Failure(RoadmapErrors.NotFound);

            roadmap.IsSaved = !roadmap.IsSaved;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling save status for roadmap {RoadmapId}", id);
            return Result.Failure(RoadmapErrors.Error);
        }
    }
}