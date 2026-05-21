namespace Career_Path.Contracts.Match;

public record MatchRequest
(
    string user_id,
    List<string> user_skills,
    List<JobMatchRequest> jobs
);