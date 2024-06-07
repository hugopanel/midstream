/*

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Application.Services;
using Api.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Data;
using BCrypt.Net;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly UserDbContext _context;

        public AccountController(UserService userService, IConfiguration configuration, UserDbContext context)
        {
            _userService = userService;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var userAlreadyExists = await _userService.CheckIfUserAlreadyExists(request.Email);
            if (userAlreadyExists)
            {
                var response = new RegisterResponse
                {
                    message = "An account already exists with this email address."
                };
                return BadRequest(response);
            }
            else
            {
                try
                {
                    var token = _userService.GenerateConfirmationToken(request.Email);
                    Console.WriteLine("token ok");
                    await _userService.SendConfirmationEmailAsync(request.Email, token);
                    Console.WriteLine("OK");
                    var response = new RegisterResponse
                    {
                        message = "Registration successful. Please check your email to confirm your account.",
                    };
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmRegister([FromQuery] string token, [FromBody] ConfirmRegisterRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var email = claimsPrincipal.FindFirst("EmailAddress")?.Value;

                var userAlreadyExists = await _userService.CheckIfUserAlreadyExists(email);

                if (userAlreadyExists)
                {
                    var response = new RegisterResponse
                    {
                        message = "An account already exists with this email address."
                    };
                    return BadRequest(response);
                }
                else
                {
                    // Create the user in the database using extracted information
                    var user = new User
                    {
                        Username = request.Username,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                        Salt = new byte[] { 0x3E, 0x4D, 0x6C, 0x2A, 0x72, 0x9F, 0x8B, 0x1C, 0x5D, 0x8E, 0x2B, 0x4F, 0x5E, 0x6A, 0x7C, 0x8D },
                        Email = email
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var response = new RegisterResponse
                    {
                        message = "You are registered ! You are redirected to the login page, please log in.",
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid token.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var userAlreadyExists = await _userService.CheckIfUserAlreadyExists(request.Email);

            if (!userAlreadyExists)
            {
                Console.WriteLine("no account");
                var response = new RegisterResponse
                {
                    message = "There is no account associated with this email address."
                };
                return BadRequest(response);
            }
            else
            {
                var isValid = await _userService.ValidateUserAsync(request.Email, request.Password);
                if (!isValid)
                {
                    var response = new RegisterResponse
                    {
                        message = "Wrong password for this email address."
                    };
                    return BadRequest(response);
                }

                // Generate JWT token
                var token = GenerateJwtToken(request.Email);
                return Ok(new { Token = token });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("forgot_password")]
        public async Task<IActionResult> Forgot_Password(ResetRequest request)
        {
            try
            {
                var token = _userService.GenerateResetPasswordToken(request.Email);
                await _userService.SendResetEmailAsync(request.Email, token);

                var response = new RegisterResponse
                {
                    message = "Please check your email to reset your password.",
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm_reset")]
        public async Task<IActionResult> ConfirmResetEmail([FromQuery] string token, [FromBody] ConfirmResetRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var email = claimsPrincipal.FindFirst("EmailAddress")?.Value;

                // Update of the database

                if (email == null)
                {
                    return BadRequest("Invalid token.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                await _context.SaveChangesAsync();

                var response = new RegisterResponse
                {
                    message = "Password changed successfully.",
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid token.");
            }
        }

        [Authorize] // Requires authentication
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var username = User.Identity.Name;

            return Ok(new { Username = username });
        }
    }
}


*/