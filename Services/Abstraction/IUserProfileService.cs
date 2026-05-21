using Career_Path.Contracts.UserProfile;

namespace Career_Path.Services
{
    public interface IUserProfileService
    {
        Task<bool> HasResumesAsync(string userId, CancellationToken ct = default);
        Task<Result<UserProfileResponse>> GetAsync(string userId, string? currentUserId = null);


        Task<Result<string>> GetProfilePictureUrlAsync(string userId);


        Task<Result> UpdateBasicInfoAsync(string UserId, BasicInfoRequest request, CancellationToken ct = default);


        Task<Result> UpdateCvAsync(string userId, UpdateUserProfileCvRequest request, CancellationToken ct = default);


        Task<Result> UpdatePictureAsync(string userId, UpdateUserProfilePictureRequest request, CancellationToken ct = default);

        Task<Result> UpdateCoverPictureAsync(string userId, UpdateUserProfileCoverRequest request, CancellationToken ct = default);

        Task<Result> DeleteCvAsync(string userId, CancellationToken ct = default);


        Task<Result> DeletePictureAsync(string userId, CancellationToken ct = default);


        Task<Result> DeleteCoverPictureAsync(string userId, CancellationToken ct = default);
    }
}
