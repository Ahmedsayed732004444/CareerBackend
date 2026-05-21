namespace Career_Path.Persistence.EntitiesConfigurations;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.HasKey(x => x.Id);

        // منع يـ follow نفسه
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_UserFollow_NotSelf",
            "\"FollowerId\" <> \"FollowingId\""));

        // منع duplicate follow
        builder.HasIndex(x => new { x.FollowerId, x.FollowingId })
            .IsUnique();
    }
}
