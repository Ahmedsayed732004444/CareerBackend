using Career_Path.Entities.AiEntities;
using Intelligent_Career_Advisor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace Career_Path.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)

{

    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobSubmission> JobSubmissions { get; set; }
    public DbSet<MembershipUpgrade> MembershipUpgrades { get; set; }
    public DbSet<RoadmapJson> RoadmapJsons { get; set; }
    public DbSet<ModelExtration> ModelExtrations { get; set; }
    public DbSet<JobInterview> JobInterviews { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<CommentReply> CommentReplies { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; }
    public DbSet<ReplyReaction> ReplyReactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<Message> Messages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }

}