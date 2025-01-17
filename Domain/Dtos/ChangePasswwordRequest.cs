namespace Singer.Domain.Dtos;

public class ChangePasswwordRequest
{
    public string Token { get; set; }
    public string Password { get; set; }
}