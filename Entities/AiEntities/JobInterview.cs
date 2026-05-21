using System.ComponentModel.DataAnnotations.Schema;

namespace Career_Path.Entities.AiEntities;

public class JobInterview
{
    public int Id { get; set; }

    public string Question { get; set; } = string.Empty;

    [ForeignKey("Job")]
    public string JobId { get; set; }

    public Job Job { get; set; } = default!;

    public ICollection<JobInterviewOption> Options { get; set; } = new List<JobInterviewOption>();
}
