namespace Singer.Domain.Dtos;

public class UserDWDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string DateRegister { get; set; }
    public bool IsActive { get; set; }
}
