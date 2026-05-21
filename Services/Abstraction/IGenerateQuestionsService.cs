namespace Career_Path.Services;

public interface IGenerateQuestionsService
{
    Task<Result> GenerateQuestionsAsync(string userId, string jobId, CancellationToken cancellationToken = default);
}
