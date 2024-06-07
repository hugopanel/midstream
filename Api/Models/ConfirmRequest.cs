namespace Api.Models;

public record ConfirmRequest(string Token, string username, string firstname, string lastname, string password);