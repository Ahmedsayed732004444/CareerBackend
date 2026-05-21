namespace Career_Path.Contracts.UserProfile
{
    public record BasicInfoRequest
        (
            string? FirstName,
            string? LastName,
            UserGender? Gender,
            string? Country,
            string? City,
            string? JobTitle,
            int? YearsOfExperience,
            string? CurrentCompany,
            string? Summary,
            string? University,
            string? Degree,
            int? GraduationYear,
            List<string> Skills
        );
}
