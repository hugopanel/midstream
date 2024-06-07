using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using MediatR;

namespace Application.Authentication.Queries;

public class LoginQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginQuery, AuthenticationResult>
{
    private IUserRepository _userRepository = userRepository;
    private IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthenticationResult> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        // Get the user
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user is null)
            throw new Exception("Invalid email."); // TODO: Create custom exception

        // Check that the password is the same
        if (!user.VerifyPassword(request.Password))
        {
            throw new Exception("Invalid password."); // TODO: Create custom exception
        }

        // Create JWT Token
        var token = _jwtTokenGenerator.GenerateLoginToken(user.Username, user.FirstName, user.LastName, user.Email);

        // Return new user
        return new AuthenticationResult(user, token);
    }
}