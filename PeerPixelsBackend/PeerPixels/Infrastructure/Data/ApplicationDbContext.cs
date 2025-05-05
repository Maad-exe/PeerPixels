using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeerPixels.Core.Entities;

namespace PeerPixels.Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the PeerPixels application.
    /// Extends the Identity DbContext to provide access to application entities.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the database set for post entities.
        /// </summary>
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>
        /// Gets or sets the database set for follow relationship entities.
        /// </summary>
        public DbSet<Follow> Follows { get; set; } = null!;

        /// <summary>
        /// Configures the model for the database context.
        /// Sets up entity relationships and constraints.
        /// </summary>
        /// <param name="builder">The builder used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure follow relationship to prevent cascading delete cycles
            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure post relationship to user
            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);
        }
    }
}