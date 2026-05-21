namespace Career_Path.Contracts.Interview;
// Request - اليوزر بيبعت إجاباته
public record SubmitInterviewRequest(
    List<UserAnswer> Answers
);

public record UserAnswer(
    int QuestionId,
    int SelectedOptionId
);

// Response - الأسئلة
public record InterviewQuestionResponse(
    int Id,
    string Question,
    List<OptionResponse> Options
);

public record OptionResponse(
    int Id,
    string OptionText
);

// Response - نتيجة التصحيح
public record InterviewResultResponse(
    int TotalQuestions,
    int CorrectAnswers,
    int Score, // percentage
    List<QuestionResultDetail> Details
);

public record QuestionResultDetail(
    int QuestionId,
    string Question,
    string YourAnswer,
    string CorrectAnswer,
    bool IsCorrect
);
