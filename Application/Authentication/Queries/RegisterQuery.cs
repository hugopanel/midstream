using MediatR;

namespace Application.Authentication.Queries;

public record RegisterQuery(
    string Email
) : IRequest<AuthenticationResult>;