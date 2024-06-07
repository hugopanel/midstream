using MediatR;

namespace Application.Authentication.Queries;

public record ResetPasswordQuery(string Email) : IRequest<AuthenticationResult>;