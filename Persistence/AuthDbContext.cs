using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetToken { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

}