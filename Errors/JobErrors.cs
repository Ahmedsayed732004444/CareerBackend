namespace Career_Path.Errors
{
    public static class JobErrors
    {
        public static readonly Error JobNotFound =
            new("Job.JobNotFound", "Job not found", StatusCodes.Status404NotFound);

        public static readonly Error Unauthorized =
            new("Job.Unauthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);

        public static readonly Error CompanyNotFound =
            new("Job.CompanyNotFound", "Company not found", StatusCodes.Status404NotFound);

        public static readonly Error InvalidJobData =
            new("Job.InvalidData", "Invalid job data provided", StatusCodes.Status400BadRequest);

        public static readonly Error JobAlreadyInactive =
            new("Job.AlreadyInactive", "Job is already inactive", StatusCodes.Status400BadRequest);
        public static readonly Error AlreadyApplied =
            new("Job.AlreadyApplied", "You have already applied to this job", StatusCodes.Status400BadRequest);
        public static readonly Error JobClosed =
            new("Job.Closed", "This job is closed and no longer accepting applications", StatusCodes.Status400BadRequest);
        public static readonly Error Error =
           new("Job.Error", "interval server Error.", StatusCodes.Status500InternalServerError);
        public static readonly Error GenerationFailed =
                new("Job.GenerationFailed", "Failed to generate job recommendations. Please try again later.", StatusCodes.Status500InternalServerError);
        public static readonly Error NoRecommendations =
                new("Job.NoRecommendations", "No job recommendations found based on your profile.", StatusCodes.Status404NotFound);
        public static readonly Error SubmissionNotFound =
                new("Job.SubmissionNotFound", "Job submission not found.", StatusCodes.Status404NotFound);


    }
}