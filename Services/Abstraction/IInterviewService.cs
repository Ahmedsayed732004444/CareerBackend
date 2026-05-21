using Career_Path.Contracts.Interview;

namespace Career_Path.Services
{
    public interface IInterviewService
    {
        Task<Result<List<InterviewQuestionResponse>>> GetInterviewQuestionsAsync(string jobId, CancellationToken cancellationToken = default);


        // صحح الإجابات
        Task<Result<InterviewResultResponse>> SubmitInterviewAsync(string jobId, SubmitInterviewRequest request, CancellationToken cancellationToken = default);

    }
}
