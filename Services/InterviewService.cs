using Career_Path.Contracts.Interview;

namespace Career_Path.Services;

public class InterviewService(
    ApplicationDbContext context,
    ILogger<InterviewService> logger) : IInterviewService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<InterviewService> _logger = logger;

    public async Task<Result<List<InterviewQuestionResponse>>> GetInterviewQuestionsAsync(
        string jobId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jobExists = await _context.Jobs
                .AnyAsync(j => j.Id == jobId, cancellationToken);

            if (!jobExists)
                return Result.Failure<List<InterviewQuestionResponse>>(JobErrors.JobNotFound);

            var questions = await _context.JobInterviews
                .Where(q => q.JobId == jobId)
                .Select(q => new InterviewQuestionResponse(
                    q.Id,
                    q.Question,
                    q.Options.Select(o => new OptionResponse(
                        o.Id,
                        o.OptionText
                    )).ToList()
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(questions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving interview questions for job {JobId}", jobId);
            return Result.Failure<List<InterviewQuestionResponse>>(JobErrors.Error);
        }
    }

    public async Task<Result<InterviewResultResponse>> SubmitInterviewAsync(
        string jobId,
        SubmitInterviewRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var questions = await _context.JobInterviews
                .Where(q => q.JobId == jobId)
                .Include(q => q.Options)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!questions.Any())
                return Result.Failure<InterviewResultResponse>(JobErrors.JobNotFound);

            var details = new List<QuestionResultDetail>();

            foreach (var question in questions)
            {
                var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var correctOption = question.Options.First(o => o.IsCorrect);
                var selectedOption = question.Options.FirstOrDefault(o => o.Id == userAnswer?.SelectedOptionId);

                details.Add(new QuestionResultDetail(
                    question.Id,
                    question.Question,
                    YourAnswer: selectedOption?.OptionText ?? "لم تجب",
                    CorrectAnswer: correctOption.OptionText,
                    IsCorrect: selectedOption?.IsCorrect ?? false
                ));
            }

            var correct = details.Count(d => d.IsCorrect);

            var result = new InterviewResultResponse(
                TotalQuestions: questions.Count,
                CorrectAnswers: correct,
                Score: (int)((double)correct / questions.Count * 100),
                Details: details
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while submitting interview for job {JobId}", jobId);
            return Result.Failure<InterviewResultResponse>(JobErrors.Error);
        }
    }
}