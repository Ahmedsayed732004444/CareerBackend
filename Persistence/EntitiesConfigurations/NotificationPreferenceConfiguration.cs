namespace Career_Path.Persistence.EntitiesConfigurations;


public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.HasKey(np => np.Id);

        // كل مستخدم عنده preference واحدة لكل type
        builder.HasIndex(np => new { np.UserId, np.Type }).IsUnique();

        builder.HasOne(np => np.User)
            .WithMany()
            .HasForeignKey(np => np.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}