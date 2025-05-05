using System;

namespace PeerPixels.Core.Entities
{
    /// <summary>
    /// Represents a follow relationship between two users in the system.
    /// This entity tracks who is following whom.
    /// </summary>
    public class Follow
    {
        /// <summary>
        /// Gets or sets the unique identifier for the follow relationship.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who is following another user.
        /// </summary>
        public string FollowerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the user who is being followed.
        /// </summary>
        public string FolloweeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the follow relationship was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to the user who is following.
        /// </summary>
        public virtual User Follower { get; set; } = null!;

        /// <summary>
        /// Navigation property to the user who is being followed.
        /// </summary>
        public virtual User Followee { get; set; } = null!;
    }
}