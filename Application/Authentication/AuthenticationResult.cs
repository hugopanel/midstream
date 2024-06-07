using Domain.Entities;
using Domain.User;

namespace Application.Authentication;

public record AuthenticationResult(
    User User,
    string Token
);