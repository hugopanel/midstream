using Domain.User;

namespace Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    User? GetUserByUsername(string username);
    
    // Va-t-on utiliser cette méthode sachant qu'elle nécessite de connaître le mot de passe hashé avec le bon salt ?
    User? GetUserByEmailAndPassword(string username, string password);
    User? GetUserByEmail(string email);
    User? GetUserById(string id);

    void Add(User user);
    void Save(User user);
}