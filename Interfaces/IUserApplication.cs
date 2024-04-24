using Singer.Domain;

namespace Singer.Interfaces;

public interface IUserApplication
{
    Task<User> CreateUserAsync(string username, string password, string email);
    Task<User> ApproveUserAsync(Guid userId);
    Task<(string key, string iv, string token)> AuthenticateUserAsync(string username, string password);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(Guid userId, string username, string email);
    Task<bool> DeleteUserAsync(Guid userId);
}
