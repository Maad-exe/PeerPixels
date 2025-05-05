using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeerPixels.Core.Entities;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for authentication operations including user registration,
    /// login, and third-party authentication through Google.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userManager">The ASP.NET Identity user manager.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="userService">The user service for retrieving user information.</param>
        public AuthService(
            UserManager<User> userManager,
            IConfiguration configuration,
            IUserService userService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object.</param>
        /// <returns>A task representing the asynchronous operation, containing the authentication response.</returns>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "Registration data is required"
                };
            }

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "User with this email already exists"
                };
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                AvatarUrl = "https://via.placeholder.com/150"
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            var userDto = await _userService.GetUserByIdAsync(user.Id, user.Id);
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Succeeded = true,
                Token = token,
                User = userDto,
                Message = "User registered successfully"
            };
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object.</param>
        /// <returns>A task representing the asynchronous operation, containing the authentication response.</returns>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "Login data is required"
                };
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "Invalid email or password"
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "Invalid email or password"
                };
            }

            var userDto = await _userService.GetUserByIdAsync(user.Id, user.Id);
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Succeeded = true,
                Token = token,
                User = userDto,
                Message = "Login successful"
            };
        }

        /// <summary>
        /// Authenticates a user using Google OAuth.
        /// </summary>
        /// <param name="googleAuthDto">The Google authentication data transfer object.</param>
        /// <returns>A task representing the asynchronous operation, containing the authentication response.</returns>
        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthDto googleAuthDto)
        {
            if (googleAuthDto == null || string.IsNullOrEmpty(googleAuthDto.IdToken))
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = "Google authentication token is required"
                };
            }

            try
            {
                // Decode the JWT token to get user info
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(googleAuthDto.IdToken);

                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
                var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name");

                if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
                {
                    return new AuthResponseDto
                    {
                        Succeeded = false,
                        Message = "Invalid Google token: email claim not found"
                    };
                }

                var email = emailClaim.Value;
                var name = nameClaim?.Value ?? email.Split('@')[0];

                // Check if user exists
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create new user
                    var userName = email.Split('@')[0];

                    // Ensure username is unique
                    var userNameExists = await _userManager.FindByNameAsync(userName);
                    if (userNameExists != null)
                    {
                        userName = $"{userName}{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                    }

                    user = new User
                    {
                        UserName = userName,
                        Email = email,
                        DisplayName = name,
                        AvatarUrl = "https://via.placeholder.com/150",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new AuthResponseDto
                        {
                            Succeeded = false,
                            Message = $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                        };
                    }
                }

                var userDto = await _userService.GetUserByIdAsync(user.Id, user.Id);
                var token = GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Succeeded = true,
                    Token = token,
                    User = userDto,
                    Message = "Google login successful"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Succeeded = false,
                    Message = $"An error occurred during Google authentication: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated JWT token as a string.</returns>
        private string GenerateJwtToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating a token");

            if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email))
                throw new InvalidOperationException("User ID, username, and email are required for token generation");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}