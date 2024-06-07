using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Application.Authentication;
using Application.Authentication.Commands;
using Application.Authentication.Queries;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Api.Controllers;

[ApiController]
[Route("Auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var command = new RegisterQuery(request.Email);
            AuthenticationResult result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            var errorMessage = new AuthenticationResponseMessage(ex.Message);
            return BadRequest(errorMessage);
        }
    }

    [HttpPost("ConfirmRegistration")]
    public async Task<IActionResult> ConfirmRegistration(ConfirmRequest request)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(request.Token);

            string username = request.username;
            string firstName = request.firstname;
            string lastName = request.lastname;
            string email = jwtSecurityToken.Claims.First(claim => claim.Type == "email").Value;

            var command = new ConfirmRegistrationCommand(username, firstName, lastName, email, request.password);
            AuthenticationResult result = await _mediator.Send(command);

            Console.WriteLine(result.ToString());

            var response = new AuthenticationResponse(result.User.Id, result.User.Username, result.User.FirstName,
                result.User.LastName, result.User.Email, result.Token);

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorMessage = new AuthenticationResponseMessage("Invalid token");
            return BadRequest(errorMessage);
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var query = new LoginQuery(request.Email, request.Password);
            AuthenticationResult result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errorMessage = new AuthenticationResponseMessage(ex.Message);
            return BadRequest(errorMessage);
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        try
        {
            var query = new ResetPasswordQuery(request.Email);
            await _mediator.Send(query);

            var message = new AuthenticationResponseMessage("Email sent successfully");

            return Ok(message);
        }
        catch (Exception ex)
        {
            var errorMessage = new AuthenticationResponseMessage("Error during the sending of the email.");
            return BadRequest(errorMessage);
        }
    }

    [HttpPost("ConfirmResetPassword")]
    public async Task<IActionResult> ConfirmResetPassword([FromQuery] string token, [FromBody] ConfirmResetPasswordRequest request)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            string id = jwtSecurityToken.Claims.First(claim => claim.Type == "userid").Value;

            var command = new ConfirmPasswordResetCommand(id, request.password);
            await _mediator.Send(command);

            var message = new AuthenticationResponseMessage("New password set successfully. Please login with your new password.");

            return Ok(message);
        }
        catch (Exception ex)
        {
            var errorMessage = new AuthenticationResponseMessage("Error during the reset of the password.");
            return BadRequest(errorMessage);
        }
    }

    [Authorize]
    [HttpGet("Profile")]
    public IActionResult Profile()
    {
        var username = User.Identity.Name;

        return Ok(new { Username = username });
    }
}