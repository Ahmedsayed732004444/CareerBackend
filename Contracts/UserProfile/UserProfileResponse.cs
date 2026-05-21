namespace Career_Path.Contracts.UserProfile
{
    public record UserProfileResponse
    (
        string FullName,
        string FirstName,
        string LastName,
        string Email,
        UserGender? Gender,
        string? JobTitle,
        string? Country,
        string? City,
        string? University,
        string? CurrentCompany,
        string? Degree,
        int? YearsOfExperience,
        string? Summary,
        int? GraduationYear,
        string? CvFileUrl,
        string? ProfilePictureUrl,
        string? CoverPictureUrl,
        ICollection<string> Skills,

        // 🆕
        int FollowersCount,
        int FollowingCount,
        bool IsFollowedByMe
    );
}