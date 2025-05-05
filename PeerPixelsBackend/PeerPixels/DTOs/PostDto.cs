using System;

namespace PeerPixels.DTOs
{
    /// <summary>
    /// Data transfer object for post information.
    /// Used when returning post data to clients.
    /// </summary>
    public class PostDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created the post.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the post creator.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the post creator.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to the avatar of the post creator.
        /// </summary>
        public string UserAvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the image associated with the post.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text caption or description of the post.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the post was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new post.
    /// Used when receiving post creation requests from clients.
    /// </summary>
    public class CreatePostDto
    {
        /// <summary>
        /// Gets or sets the URL of the image to be associated with the new post.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text caption or description of the new post.
        /// </summary>
        public string Caption { get; set; } = string.Empty;
    }
}