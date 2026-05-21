namespace Career_Path.Contracts.Match;

public record JobMatchRequest
(
   string job_id,
   string job_title,
   string job_description,
   List<string> job_skills
);
