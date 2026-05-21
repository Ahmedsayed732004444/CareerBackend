using System.ComponentModel.DataAnnotations.Schema;

namespace Career_Path.Entities.AiEntities;

public class RoadmapJson
{
    public int Id { get; set; }
    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public string RoadmapData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSaved { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}
