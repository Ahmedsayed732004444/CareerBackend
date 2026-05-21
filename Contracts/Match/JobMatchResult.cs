namespace Career_Path.Contracts.Match;

public record JobMatchResult
(
    string job_id,
    string job_title,
    double match_percentage,
    List<string> matched_skills,
    List<string> missing_skills
);

