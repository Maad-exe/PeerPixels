namespace PeerPixels.DTOs
{
    /// <summary>
    /// Data transfer object for user information.
    /// Used when returning user data to clients.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the user shown on their profile.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to the user's profile picture.
        /// </summary>
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the count of users who are following this user.
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// Gets or sets the count of users that this user is following.
        /// </summary>
        public int FollowingCount { get; set; }

        /// <summary>
        /// Gets or sets the count of posts created by this user.
        /// </summary>
        public int PostsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user is following this user.
        /// </summary>
        public bool IsFollowing { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating user information.
    /// Used when receiving user update requests from clients.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Gets or sets the updated display name for the user.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated URL to the user's profile picture.
        /// </summary>
        public string AvatarUrl { get; set; } = string.Empty;
    }
}