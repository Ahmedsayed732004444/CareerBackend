namespace Career_Path.Errors
{
    public record MatchErrors
    {
        public static readonly Error NoSkills =
            new("NoSkills", "No skills found for the user.", StatusCodes.Status404NotFound);
        public static readonly Error NoJobs =
            new("NoJobs", "No matching jobs found for the user's skills.", StatusCodes.Status404NotFound);
        public static readonly Error MatchFailed =
            new("MatchFailed", "Failed to get job matches. Please try again later.", StatusCodes.Status500InternalServerError);
        public static readonly Error UploadAFalidCVFile =
            new("UploadAFalidCVFile", "Please upload a valid CV file to get job matches.", StatusCodes.Status400BadRequest);
    }
}
