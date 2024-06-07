using Application.Common.Interfaces;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Domain.User;
using MediatR;

namespace Application.Authentication.Queries;

public class RegisterQueryHandler : IRequestHandler<RegisterQuery, AuthenticationResult>
{
    private IUserRepository _userRepository;
    private IJwtTokenGenerator _jwtTokenGenerator;
    private IEmailService _emailService;

    public RegisterQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailService = emailService;
    }

    public async Task<AuthenticationResult> Handle(RegisterQuery query, CancellationToken cancellationToken)
    {
        // Check if user with given name already exists
        if (_userRepository.GetUserByEmail(query.Email) is not null)
            throw new Exception("User already exists."); // TODO: Create custom exception

        User newUser = new User
        {
            Email = query.Email
        };

        // Create JWT Token
        var token = _jwtTokenGenerator.GenerateRegistrationToken(query.Email);

        // Send confirmation email
        await _emailService.SendConfirmationEmailAsync(query.Email, token);

        // Return new user
        return new AuthenticationResult(newUser, token);
    }
}