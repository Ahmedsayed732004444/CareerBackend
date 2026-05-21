
namespace Career_Path.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public UserProfile? UserProfile { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<Job> PostedJobs { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<PostLike> PostLikes { get; set; } = [];
    public List<JobSubmission> JobSubmissions { get; set; } = [];
    public ICollection<PostComment> PostComments { get; set; } = [];
    public ICollection<CommentReply> CommentReplies { get; set; } = [];
    public ICollection<CommentReaction> CommentReactions { get; set; } = [];
    public ICollection<ReplyReaction> ReplyReactions { get; set; } = [];
    // الناس اللي أنا بـ follow هم
    public ICollection<UserFollow> Following { get; set; } = [];

    // الناس اللي بيـ follow أنا
    public ICollection<UserFollow> Followers { get; set; } = [];
}