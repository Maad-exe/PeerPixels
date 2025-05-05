using System;

namespace PeerPixels.Core.Entities
{
    /// <summary>
    /// Represents a user post in the social media platform.
    /// Contains the post content and metadata.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Gets or sets the unique identifier for the post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created the post.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the post was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to the user who created the post.
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}