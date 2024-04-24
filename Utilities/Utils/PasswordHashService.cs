
using System.Security.Cryptography;
using System.Text;

namespace Singer.Utilities.Utils;

public class PasswordHashService
{
    public string GenerateHash(string password)
    {
        byte[] salt = GenerateSalt();

        string hash = HashPassword(password, salt);

        return Convert.ToBase64String(salt) + "|" + hash;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            string[] parts = hashedPassword.Split("|");
            byte[] salt = Convert.FromBase64String(parts[0]);
            string storeHash = parts[1];

            string computedHash = HashPassword(password, salt);

            return computedHash == storeHash;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string HashPassword(string password, byte[] salt)
    {
        byte[] combinedBytes = new byte[salt.Length + password.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(password), 0, combinedBytes, salt.Length, password.Length);

        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }
}
