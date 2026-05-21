namespace Career_Path.Errors
{
    public record FollowErrors
    {
        public static readonly Error Error =
       new("Follow.Error", "An error occurred while processing the follow", StatusCodes.Status500InternalServerError);

        public static readonly Error AlreadyFollowing =
            new("Follow.AlreadyFollowing", "You are already following this user", StatusCodes.Status400BadRequest);

        public static readonly Error NotFollowing =
            new("Follow.NotFollowing", "You are not following this user", StatusCodes.Status404NotFound);

        public static readonly Error CannotFollowYourself =
            new("Follow.CannotFollowYourself", "You cannot follow yourself", StatusCodes.Status400BadRequest);
    }
}
