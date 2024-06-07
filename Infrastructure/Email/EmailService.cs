using System.Net;
using System.Net.Mail;
using Application.Common.Interfaces.Email;

namespace Infrastructure.Email;

public class EmailService : IEmailService
{
    public async Task SendConfirmationEmailAsync(string email, string token)
    {
        var fromAddress = new MailAddress("midstream42@gmail.com", "Midstream");
        var toAddress = new MailAddress(email);
        var fromPassword = "njoewmtozlrhoyzl";
        var subject = "Confirm Your Email";
        var body =
            $"Click the following link to confirm your email: http://localhost:3000/register_two?token={token}";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = subject;
        message.Body = body;
        await smtp.SendMailAsync(message);
    }

    public async Task SendResetPasswordEmailAsync(string email, string token)
    {
        var fromAddress = new MailAddress("midstream42@gmail.com", "Midstream");
        var toAddress = new MailAddress(email);
        var fromPassword = "njoewmtozlrhoyzl";
        var subject = "Reset your password";
        var body =
            $"Click the following link to reset your password: {token}";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress);
        message.Subject = subject;
        message.Body = body;
        await smtp.SendMailAsync(message);
    }
}