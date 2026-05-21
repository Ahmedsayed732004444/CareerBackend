namespace Career_Path.Services;

public class GenerateQuestionsService : IGenerateQuestionsService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ApplicationDbContext _context;

    public GenerateQuestionsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ApplicationDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "https://final-ai-project-last-version-production.up.railway.app/";
        //_baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "http://127.0.0.1:8000/";
        this._context = context;
    }

    public async Task<Result> GenerateQuestionsAsync(string userId, string jobId, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == userId, cancellationToken);

        if (job is null)
            return Result.Failure(JobErrors.JobNotFound);

        try
        {
            var jobRequest = new
            {
                job = new
                {
                    job_id = job.Id,
                    job_title = job.JobTitle,
                    job_description = job.JobDescription,
                    job_skills = job.JobRequirements
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}interview-questions", jobRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Result.Failure(JobErrors.GenerationFailed);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(JobErrors.GenerationFailed);
        }
    }

}
