using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Domain.User;
using MediatR;

namespace Application.Authentication.Queries;

public class ResetPasswordQueryHandler : IRequestHandler<ResetPasswordQuery, AuthenticationResult>
{
    private IEmailService _emailService;
    private IJwtTokenGenerator _jwtTokenGenerator;
    private IUserRepository _userRepository;

    public ResetPasswordQueryHandler(IEmailService emailService, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _emailService = emailService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<AuthenticationResult> Handle(ResetPasswordQuery request, CancellationToken cancellationToken)
    {
        // Check if the user exists
        User? user = _userRepository.GetUserByEmail(request.Email);
        if (user is null)
            throw new Exception("No user exists with this email address."); // TODO: Create custom exception

        // Generate a token
        var token = _jwtTokenGenerator.GeneratePasswordResetToken(user.Id);

        // Send email with reset password link
        await _emailService.SendResetPasswordEmailAsync(request.Email, token);

        // Return new user
        return new AuthenticationResult(user, token);
    }
}