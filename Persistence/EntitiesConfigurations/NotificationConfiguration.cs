namespace Career_Path.Persistence.EntitiesConfigurations;


public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.EntityType).HasMaxLength(100);
        builder.Property(n => n.EntityId).HasMaxLength(100);

        builder.HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Actor)
            .WithMany()
            .HasForeignKey(n => n.ActorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Index لتسريع استعلامات الـ user
        builder.HasIndex(n => new { n.RecipientId, n.IsRead, n.CreatedAt });
    }
}
