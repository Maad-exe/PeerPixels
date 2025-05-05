using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PeerPixels.Core.Entities
{
    /// <summary>
    /// Represents a user in the PeerPixels platform.
    /// Extends the ASP.NET Identity User class with additional properties specific to the application.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the user's display name shown on their profile and posts.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to the user's profile picture.
        /// </summary>
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the user profile was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to the collection of posts created by this user.
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        /// <summary>
        /// Navigation property to the collection of follows where this user is being followed.
        /// </summary>
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

        /// <summary>
        /// Navigation property to the collection of follows where this user is following others.
        /// </summary>
        public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}