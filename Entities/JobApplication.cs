namespace Intelligent_Career_Advisor.Models;

public class JobApplication
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public string? ApplicationSource { get; set; }  // For example: LinkedIn, Company Website, Referral
    public string? Notes { get; set; }

    // Foreign key to ApplicationUser
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = default!;

}
public enum ApplicationStatus
{
    Applied,
    Interviewed,
    Offered,
    Rejected,
    Accepted
}
