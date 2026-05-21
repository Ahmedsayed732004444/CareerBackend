using Career_Path.Contracts.Match;
using CareerPathFinal.Services;
using System.Text;
using System.Text.Json;

namespace Career_Path.Services;


public class MatchService : IMatchService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ApplicationDbContext _context;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MatchService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ApplicationDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient();
        // _baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "http://127.0.0.1:8000/";
        _baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "https://final-ai-project-last-version-production.up.railway.app/";
        _context = context;
    }

    public async Task<Result<List<JobMatchResult>>> GetMatchAsync(string userId, CancellationToken cancellationToken)
    {
        var hasFile = await _context.UserProfiles.
            Where(x => x.UserId == userId && x.CvFileUrl != null).AnyAsync(cancellationToken);
        if (!hasFile)
            return Result.Failure<List<JobMatchResult>>(MatchErrors.UploadAFalidCVFile);
        // ── 1. استدعي الـ SP ───────────────────────────────────────────────────
        var rows = await _context.Database
            .SqlQuery<JobMatchDto>($"EXEC GetTop10MatchingJobs @UserId = {userId}")
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return Result.Failure<List<JobMatchResult>>(MatchErrors.NoJobs);

        // ── 2. جيب الـ user skills للـ AI request ─────────────────────────────
        var userSkills = await _context.ModelExtrations
            .Where(x => x.ApplicationUserId == userId)
            .Select(m => m.Skills)
            .FirstOrDefaultAsync(cancellationToken);

        if (userSkills is null || userSkills.Count == 0)
            return Result.Failure<List<JobMatchResult>>(MatchErrors.NoSkills);

        // ── 3. Map SP result لـ JobMatchRequest ───────────────────────────────
        var topJobs = rows
            .Select(r => new JobMatchRequest(
                job_id: r.job_id,
                job_title: r.job_title,
                job_description: r.job_description,
                job_skills: JsonSerializer.Deserialize<List<string>>(r.job_skills_json) ?? []
            ))
            .ToList();

        // ── 4. Call Python AI Service → /match ────────────────────────────────
        try
        {
            var matchRequest = new MatchRequest(userId, userSkills, topJobs);
            var json = JsonSerializer.Serialize(matchRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}match", content, cancellationToken);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return Result.Failure<List<JobMatchResult>>(MatchErrors.MatchFailed);

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var matchResponse = JsonSerializer.Deserialize<MatchResponse>(responseJson, _jsonOptions);

            if (matchResponse is null)
                return Result.Failure<List<JobMatchResult>>(MatchErrors.MatchFailed);

            // ── 5. Map to JobMatchResult ───────────────────────────────────────
            var results = matchResponse.results
                 .Select(r => new JobMatchResult(
                     job_id: r.job_id,
                     job_title: r.job_title,
                     match_percentage: r.match_percentage,
                     matched_skills: r.matched_skills,
                     missing_skills: r.missing_skills
                 ))
                 .ToList();

            return Result.Success(results);
        }
        catch (Exception)
        {
            return Result.Failure<List<JobMatchResult>>(MatchErrors.MatchFailed);
        }
    }
}