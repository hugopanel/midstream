using Domain.Entities;
using Domain.User;

namespace Application.Authentication;

public record AuthenticationError(
    string message
);