using Microsoft.AspNetCore.Mvc;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using System.Threading.Tasks;

namespace PeerPixels.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication-related endpoints
    /// including user registration, login, and third-party authentication.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service for handling auth operations.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object containing user registration details.</param>
        /// <returns>An action result containing the registration outcome.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing user credentials.</param>
        /// <returns>An action result containing the login outcome with auth token if successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Handles Google OAuth authentication flow.
        /// </summary>
        /// <param name="googleAuthDto">The Google authentication data transfer object containing the OAuth token.</param>
        /// <returns>An action result containing the Google authentication outcome.</returns>
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin(GoogleAuthDto googleAuthDto)
        {
            var result = await _authService.GoogleLoginAsync(googleAuthDto);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}