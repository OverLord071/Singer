using Microsoft.EntityFrameworkCore;
using Singer.Domain;

namespace Singer.Infrastructure;

public class SignerDbContext : DbContext
{
    public SignerDbContext(DbContextOptions<SignerDbContext> options) : base(options)
    {
    }
    public DbSet<User>  Users { get; set; }

    public DbSet<UserDW> UsersDw { get; set; }

    public DbSet<DocumentDW> Documents { get; set; }
}
