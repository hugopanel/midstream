using Domain.Entities;
using Domain.User.ValueObjects;

namespace Domain.User
{
    public class User
    {
        // Backing fields
        private Guid _id;
        private string _username;
        private string _firstName;
        private string _lastName;
        private Password _password;
        private string _email;
        private List<Role> _roles;

        // Properties
        public Guid Id { get => _id; init => _id = value; }
        public string Username { get => _username; init => _username = value; }
        public string FirstName { get => _firstName; init => _firstName = value; }
        public string LastName { get => _lastName; init => _lastName = value; }
        public Password Password { get => _password; init => _password = value; }
        public string Email { get => _email; init => _email = value; }
        public List<Role> Roles { get => _roles; init => _roles = value; }

        public void ChangePassword(Password newPassword) => _password = newPassword;
        public void ChangePassword(string newPlainTextPassword) => _password = Password.FromPlainText(newPlainTextPassword);
        public bool VerifyPassword(string plainTextPassword) => _password.Verify(plainTextPassword);
    }
}