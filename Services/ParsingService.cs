using CareerPathFinal.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Career_Path.Services;

public class ParsingService : IParsingService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ParsingService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "https://final-ai-project-last-version-production.up.railway.app/";
        // _baseUrl = configuration["ExtractionApi:BaseUrl"] ?? "http://127.0.0.1:8000/";

    }

    public async Task<Result<string>> GetExtractionAsync(string userId, IFormFile formFile, CancellationToken cancellationToken)
    {
        try
        {
            using var formData = new MultipartFormDataContent();
            using var fileStream = formFile.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            formData.Add(fileContent, "file", formFile.FileName);
            formData.Add(new StringContent(userId), "application_user_id");

            var response = await _httpClient.PostAsync($"{_baseUrl}cv-box", formData, cancellationToken);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return Result.Failure<string>(ParsingErrors.ParsingFailed);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            var cvReview = data.TryGetProperty("cv_review", out var prop) ? prop.GetString() ?? string.Empty : string.Empty;

            return Result.Success(cvReview);
        }
        catch (Exception)
        {
            return Result.Failure<string>(ParsingErrors.ParsingFailed);
        }
    }
}
