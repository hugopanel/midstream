namespace Api.Models;

public record ConfirmResetPasswordRequest(string Token, string NewPassword);
