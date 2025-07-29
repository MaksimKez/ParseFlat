using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence;

public class AuthDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<EmailVerificationTokenEntity> EmailVerificationTokens { get; set; }
    public DbSet<PasswordResetTokenEntity> PasswordResetToken { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

}