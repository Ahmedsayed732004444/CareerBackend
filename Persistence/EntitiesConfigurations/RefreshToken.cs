namespace Career_Path.Persistence.EntitiesConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens"); // ✅ نفس اسم الجدول القديم

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();
    }
}