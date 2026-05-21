using Career_Path.Contracts.Common;
using Career_Path.Contracts.Posts;
using Career_Path.Services.Abstraction;

namespace Career_Path.Services;

public class PostService(
    ApplicationDbContext context,
    ILogger<PostService> logger,
    IWebHostEnvironment env,
    IHttpContextAccessor accessor) : IPostService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<PostService> _logger = logger;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _accessor = accessor;

    public async Task<Result<PostResponse>> CreatePostAsync(string userId, CreatePostRequest request, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _context.UserProfiles
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.ApplicationUser.FullName,
                    p.JobTitle,
                    p.ProfilePictureUrl,
                    p.Country
                })
                .FirstOrDefaultAsync(ct);

            if (userProfile is null)
                return Result.Failure<PostResponse>(UserErrors.ProfileNotFound);

            var post = new Post
            {
                Id = Guid.CreateVersion7().ToString(),
                Content = request.Content,
                UserId = userId
            };

            if (request.File is not null)
                post.FileUrl = await FileHelper.UploadeFileAsync(request.File, "Posts", _env, _accessor);

            await _context.Posts.AddAsync(post, ct);
            await _context.SaveChangesAsync(ct);

            var newPost = new PostResponse(
                post.Id,
                post.Content,
                post.FileUrl,
                post.CreatedAt,
                0,
                false,
                new PostAuthorSummary(userId, userProfile.FullName, userProfile.JobTitle, userProfile.ProfilePictureUrl, userProfile.Country)
            );

            return Result.Success(newPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post for user {UserId}", userId);
            return Result.Failure<PostResponse>(PostErrors.Error);
        }
    }

    public async Task<Result> SoftDeletePostAsync(string userId, string postId, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == postId && !p.IsDeleted, ct);

            if (post is null)
                return Result.Failure(PostErrors.PostNotFound);

            if (!string.IsNullOrEmpty(post.FileUrl))
                FileHelper.DeleteFile(post.FileUrl, "Posts", _env);

            post.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post {PostId} for user {UserId}", postId, userId);
            return Result.Failure(PostErrors.Error);
        }
    }

    public async Task<Result> UpdatePostAsync(string userId, string postId, UpdatePostRequest request, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == postId && !p.IsDeleted, ct);

            if (post is null)
                return Result.Failure(PostErrors.PostNotFound);

            post.Content = request.Content;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post {PostId} for user {UserId}", postId, userId);
            return Result.Failure(PostErrors.Error);
        }
    }
    public async Task<Result<PostResponse>> GetPostAsync(string postId, string? userId = null, CancellationToken ct = default)
    {
        try
        {
            var postResponse = await _context.Posts
                .Where(p => !p.IsDeleted && p.Id == postId)
                .Select(post => new PostResponse(
                    post.Id,
                    post.Content,
                    post.FileUrl,
                    post.CreatedAt,
                    post.Likes.Count,
                    post.Likes.Any(l => l.UserId == userId),
                    new PostAuthorSummary(
                        post.UserId,
                        post.User.FullName,
                        post.User.UserProfile == null ? null : post.User.UserProfile.JobTitle,
                        post.User.UserProfile == null ? null : post.User.UserProfile.ProfilePictureUrl,
                        post.User.UserProfile == null ? null : post.User.UserProfile.Country)
                ))
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (postResponse is null)
                return Result.Failure<PostResponse>(PostErrors.PostNotFound);

            return Result.Success(postResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving post {PostId}", postId);
            return Result.Failure<PostResponse>(PostErrors.Error);
        }
    }

    public async Task<PaginatedList<PostResponse>> GetPostsByUserAsync(string userId, string? userSearchId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var currentUserId = userSearchId ?? userId;
            var query = _context.Posts
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .AsSplitQuery()
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Content != null && x.Content.Contains(filters.SearchValue!))
                .Select(post => new PostResponse(
                    post.Id,
                    post.Content,
                    post.FileUrl,
                    post.CreatedAt,
                    post.Likes.Count,
                    post.Likes.Any(l => l.UserId == currentUserId),
                    new PostAuthorSummary(
                        post.UserId,
                        post.User.FullName,
                        post.User.UserProfile == null ? null : post.User.UserProfile.JobTitle,
                        post.User.UserProfile == null ? null : post.User.UserProfile.ProfilePictureUrl,
                        post.User.UserProfile == null ? null : post.User.UserProfile.Country)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving posts for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedList<PostResponse>> GetPostsAsync(string? userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Posts
                .Where(p => !p.IsDeleted)
                .AsSplitQuery()
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Content != null && x.Content.Contains(filters.SearchValue!))
                .Select(post => new PostResponse(
                    post.Id,
                    post.Content,
                    post.FileUrl,
                    post.CreatedAt,
                    post.Likes.Count,
                    post.Likes.Any(l => l.UserId == userId),
                    new PostAuthorSummary(
                        post.UserId,
                        post.User.FullName,
                        post.User.UserProfile == null ? null : post.User.UserProfile.JobTitle,
                        post.User.UserProfile == null ? null : post.User.UserProfile.ProfilePictureUrl,
                        post.User.UserProfile == null ? null : post.User.UserProfile.Country)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all posts");
            throw;
        }
    }
}