namespace PeerPixels.DTOs
{
    /// <summary>
    /// Data transfer object for user login requests.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the email address for authentication.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for user registration requests.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Gets or sets the unique username for the new user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the new user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the new user.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name for the new user.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for authentication response.
    /// Contains the result of authentication operations.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation was successful.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the JWT token for authenticated sessions.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the authenticated user's information.
        /// </summary>
        public UserDto User { get; set; } = null!;

        /// <summary>
        /// Gets or sets a message describing the result of the authentication operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for Google OAuth authentication.
    /// </summary>
    public class GoogleAuthDto
    {
        /// <summary>
        /// Gets or sets the Google OAuth ID token for authentication.
        /// </summary>
        public string IdToken { get; set; } = string.Empty;
    }
}