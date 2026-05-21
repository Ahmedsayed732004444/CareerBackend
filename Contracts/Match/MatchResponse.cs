namespace Career_Path.Contracts.Match;

public record MatchResponse
(
    string user_id,
    int total_jobs,
    int prompt_db_id,
    bool generation_failed,
    List<JobMatchResult> results
);


