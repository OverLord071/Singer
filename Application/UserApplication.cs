using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Singer.Domain;
using Singer.Infrastructure;
using Singer.Interfaces;
using Singer.Utilities.Utils;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Singer.Application;

public class UserApplication : IUserApplication
{
    private readonly SignerDbContext signerDbContext;
    private readonly JwtService jwtService;
    private readonly PasswordHashService passwordHashService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserApplication(SignerDbContext signerDbContext, JwtService jwtService, PasswordHashService passwordHashService, IHttpContextAccessor httpContextAccessor)
    {
        this.signerDbContext = signerDbContext;
        this.jwtService = jwtService;
        this.passwordHashService = passwordHashService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User> ApproveUserAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.IsAprroved)
            throw new InvalidOperationException("Usuario ya aprobado");

        user.IsAprroved = true;

        using (var aes = Aes.Create()) 
        {
            aes.GenerateKey();
            aes.GenerateIV();

            user.EncryptionKey = Convert.ToBase64String(aes.Key);
            user.IV = Convert.ToBase64String(aes.IV);
        }

        await signerDbContext.SaveChangesAsync();

        return user;
    }

    public async Task<(string key, string iv, string token)> AuthenticateUserAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new ArgumentException("Nombre de usuario y contraseña son requeridos.");

        var user = await signerDbContext.Users.SingleOrDefaultAsync(u => u.Name == username);
        if (user == null)
            throw new UnauthorizedAccessException("Usuario o contraseña invalidad.");

        if (!user.IsAprroved)
            throw new UnauthorizedAccessException("Usuario no aprobado.");

        if (!passwordHashService.VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Usuario o contraseña inválidos.");

        var token = jwtService.GenerateToken(user);

        return (user.EncryptionKey, user.IV, token);
    }

    public async Task<User> CreateUserAsync(string username, string password, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            throw new ArgumentNullException("Nombre de usuario, contraseña y email son requeridos.");

        if (await signerDbContext.Users.AnyAsync(u => u.Name == username))
            throw new InvalidOperationException("Nombre de usuario ya registrado");

        var passwordHash = passwordHashService.GenerateHash(password);

        var user = new User
        {
            Name = username,
            PasswordHash = passwordHash,
            Email = email,
            Role = "User",
            IsAprroved = false,
            EncryptionKey = "",
            IV = ""
        };

        signerDbContext.Users.Add(user);
        await signerDbContext.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        signerDbContext.Users.Remove(user);
        await signerDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await signerDbContext.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await signerDbContext.Users.FindAsync(userId);
    }

    public async Task<User> UpdateUserAsync(Guid userId, string username, string email)
    {
        var user = await GetUserByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            throw new ArgumentException("Nombre de usuario y email son requeridos");

        user.Name = username;
        user.Email = email;

        await signerDbContext.SaveChangesAsync();

        return user;
    }
}
