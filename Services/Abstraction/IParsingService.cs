namespace CareerPathFinal.Services;

public interface IParsingService
{
    Task<Result<string>> GetExtractionAsync(string userId, IFormFile formFile, CancellationToken cancellationToken);
}
