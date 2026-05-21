namespace Career_Path.Entities;

public class RefreshToken
{
    public int Id { get; set; }  // ✅ أضف Primary Key
    public string UserId { get; set; } = string.Empty;  // ✅ أضف FK صريح

    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => RevokedOn is null && !IsExpired;
}