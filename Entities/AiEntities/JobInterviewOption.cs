namespace Career_Path.Entities.AiEntities;

[Owned]
public class JobInterviewOption
{
    public int Id { get; set; }

    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
