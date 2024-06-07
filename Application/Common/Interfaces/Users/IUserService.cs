using Domain.User;

namespace Application.Common.Interfaces.Users;

public interface IUserService
{
    public void ChangeUserPassword(User user, string newPassword);
}