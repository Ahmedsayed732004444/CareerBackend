using Career_Path.Contracts.UserProfile;

namespace Career_Path.Services
{
    public class UserProfileService(
        ApplicationDbContext context,
        ILogger<UserProfileService> logger,
        IWebHostEnvironment env,
        IHttpContextAccessor accessor) : IUserProfileService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<UserProfileService> _logger = logger;
        private readonly IHttpContextAccessor _accessor = accessor;
        private readonly IWebHostEnvironment _env = env;

        public async Task<bool> HasResumesAsync(string userId, CancellationToken ct = default)
        {
            try
            {
                return await _context.UserProfiles
                    .AnyAsync(up => up.UserId == userId && !string.IsNullOrWhiteSpace(up.CvFileUrl), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking resumes for user {UserId}", userId);
                return false;
            }
        }

        public async Task<Result<UserProfileResponse>> GetAsync(string userId, string? currentUserId = null)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .Where(p => p.UserId == userId)
                    .Select(p => new UserProfileResponse(
                        p.ApplicationUser.FullName,
                        p.ApplicationUser.FirstName,
                        p.ApplicationUser.LastName,
                        p.ApplicationUser.Email!,
                        p.Gender,
                        p.JobTitle,
                        p.Country,
                        p.City,
                        p.University,
                        p.CurrentCompany,
                        p.Degree,
                        p.YearsOfExperience,
                        p.Summary,
                        p.GraduationYear,
                        p.CvFileUrl,
                        p.ProfilePictureUrl,
                        p.CoverPictureUrl,
                        p.Skills,

                        // 🆕
                        p.ApplicationUser.Followers.Count,
                        p.ApplicationUser.Following.Count,
                        p.ApplicationUser.Followers.Any(f => f.FollowerId == currentUserId)
                    ))
                    .FirstOrDefaultAsync();

                if (profile is null)
                    return Result.Failure<UserProfileResponse>(UserErrors.ProfileNotFound);

                return Result.Success(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile for user {UserId}", userId);
                return Result.Failure<UserProfileResponse>(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result<string>> GetProfilePictureUrlAsync(string userId)
        {
            try
            {
                var pictureUrl = await _context.UserProfiles
                    .Where(up => up.UserId == userId)
                    .Select(up => up.ProfilePictureUrl)
                    .FirstOrDefaultAsync();

                if (pictureUrl is null)
                    return Result.Failure<string>(UserErrors.ProfileNotFound);

                return Result.Success(pictureUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile picture for user {UserId}", userId);
                return Result.Failure<string>(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> UpdateBasicInfoAsync(string userId, BasicInfoRequest request, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .Include(up => up.ApplicationUser)
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                request.Adapt(userProfile);

                if (request.FirstName is not null)
                    userProfile.ApplicationUser.FirstName = request.FirstName;

                if (request.LastName is not null)
                    userProfile.ApplicationUser.LastName = request.LastName;

                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating basic info for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> UpdateCvAsync(string userId, UpdateUserProfileCvRequest request, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (!string.IsNullOrEmpty(userProfile.CvFileUrl))
                    FileHelper.DeleteFile(userProfile.CvFileUrl, "CvS", _env);

                userProfile.CvFileUrl = await FileHelper.UploadeFileAsync(request.CvFile, "CvS", _env, _accessor);
                await _context.SaveChangesAsync(ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating CV for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> UpdatePictureAsync(string userId, UpdateUserProfilePictureRequest request, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (!string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
                    FileHelper.DeleteFile(userProfile.ProfilePictureUrl, "Images", _env);

                userProfile.ProfilePictureUrl = await FileHelper.UploadeFileAsync(request.ProfilePicture, "Images", _env, _accessor);
                await _context.SaveChangesAsync(ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile picture for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> UpdateCoverPictureAsync(string userId, UpdateUserProfileCoverRequest request, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (!string.IsNullOrEmpty(userProfile.CoverPictureUrl))
                    FileHelper.DeleteFile(userProfile.CoverPictureUrl, "Images", _env);

                userProfile.CoverPictureUrl = await FileHelper.UploadeFileAsync(request.CoverPicture, "Images", _env, _accessor);
                await _context.SaveChangesAsync(ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cover picture for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> DeleteCvAsync(string userId, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (userProfile.CvFileUrl is null)
                    return Result.Failure(UserErrors.FileNotFound);

                FileHelper.DeleteFile(userProfile.CvFileUrl, "CvS", _env);
                userProfile.CvFileUrl = null;

                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting CV for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> DeletePictureAsync(string userId, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (userProfile.ProfilePictureUrl is null)
                    return Result.Failure(UserErrors.FileNotFound);

                FileHelper.DeleteFile(userProfile.ProfilePictureUrl, "Images", _env);
                userProfile.ProfilePictureUrl = null;

                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting profile picture for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }

        public async Task<Result> DeleteCoverPictureAsync(string userId, CancellationToken ct = default)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId, ct);

                if (userProfile is null)
                    return Result.Failure(UserErrors.ProfileNotFound);

                if (userProfile.CoverPictureUrl is null)
                    return Result.Failure(UserErrors.FileNotFound);

                FileHelper.DeleteFile(userProfile.CoverPictureUrl, "Images", _env);
                userProfile.CoverPictureUrl = null;

                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting cover picture for user {UserId}", userId);
                return Result.Failure(UserErrors.UnexpectedError);
            }
        }
    }
}