namespace Application.Common.Interfaces.Email;

public interface IEmailService
{
    public Task SendConfirmationEmailAsync(string email, string token);
    public Task SendResetPasswordEmailAsync(string email, string token);
}