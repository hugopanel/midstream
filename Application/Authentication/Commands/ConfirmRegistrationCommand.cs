using MediatR;

namespace Application.Authentication.Commands;

public record ConfirmRegistrationCommand(
    string username,
    string firstName,
    string lastName,
    string email,
    string password
) : IRequest<AuthenticationResult>;