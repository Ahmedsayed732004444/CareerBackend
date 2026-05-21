namespace Career_Path.Contracts.Match;

public class JobMatchDto
{
    public string job_id { get; set; } = string.Empty;
    public string job_title { get; set; } = string.Empty;
    public string job_description { get; set; } = string.Empty;
    public string job_skills_json { get; set; } = string.Empty;
    public int MatchCount { get; set; }
}
