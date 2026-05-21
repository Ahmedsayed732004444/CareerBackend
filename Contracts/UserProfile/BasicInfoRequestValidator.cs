namespace Career_Path.Contracts.UserProfile
{
    public class BasicInfoRequestValidator : AbstractValidator<BasicInfoRequest>
    {
        public BasicInfoRequestValidator()
        {
            // ── Strings ──────────────────────────────────────────
            RuleFor(x => x.FirstName)
                .Length(3, 100)
                .When(x => x.FirstName is not null);

            RuleFor(x => x.LastName)
                .Length(3, 100)
                .When(x => x.LastName is not null);

            RuleFor(x => x.Country)
                .Length(3, 100)
                .When(x => x.Country is not null);

            RuleFor(x => x.City)
                .Length(3, 100)
                .When(x => x.City is not null);

            RuleFor(x => x.JobTitle)
                .Length(3, 200)
                .When(x => x.JobTitle is not null);

            RuleFor(x => x.CurrentCompany)
                .Length(3, 200)
                .When(x => x.CurrentCompany is not null);

            RuleFor(x => x.Summary)
                .Length(10, 1000)
                .When(x => x.Summary is not null);

            RuleFor(x => x.University)
                .Length(3, 200)
                .When(x => x.University is not null);

            RuleFor(x => x.Degree)
                .Length(2, 100)
                .When(x => x.Degree is not null);

            // ── Enum ─────────────────────────────────────────────
            RuleFor(x => x.Gender)
                .IsInEnum()
                .When(x => x.Gender is not null);

            // ── Ints ─────────────────────────────────────────────
            RuleFor(x => x.YearsOfExperience)
                .InclusiveBetween(0, 50)
                .When(x => x.YearsOfExperience is not null);

            RuleFor(x => x.GraduationYear)
                .InclusiveBetween(1950, DateTime.UtcNow.Year)
                .When(x => x.GraduationYear is not null);

            // ── Collections ───────────────────────────────────────
            RuleFor(x => x.Skills)
                .Must(s => s.Count <= 20)
                .WithMessage("Skills cannot exceed 20 items.")
                .When(x => x.Skills is { Count: > 0 });

            RuleForEach(x => x.Skills)
                .Length(2, 50)
                .When(x => x.Skills is { Count: > 0 });
        }
    }
}