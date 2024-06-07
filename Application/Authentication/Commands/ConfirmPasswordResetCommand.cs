using MediatR;

namespace Application.Authentication.Commands;

public record ConfirmPasswordResetCommand(string Id, string NewPassword) : IRequest<Unit>;