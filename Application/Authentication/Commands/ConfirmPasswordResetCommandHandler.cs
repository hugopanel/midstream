using Application.Common.Interfaces.Persistence;
using Domain.User;
using MediatR;

namespace Application.Authentication.Commands;

public class ConfirmPasswordResetCommandHandler : IRequestHandler<ConfirmPasswordResetCommand, Unit>
{
    private IUserRepository _userRepository;

    public ConfirmPasswordResetCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
    {
        // Get the user
        User? user = _userRepository.GetUserById(request.Id);
        if (user is null)
            throw new Exception("User not found."); // TODO: Create custom exception

        // Change the password of the user
        user.ChangePassword(request.NewPassword);

        return Unit.Value; // Return Unit 
    }
}