using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Features.Auth.Models;
using UserManagement.Api.Features.Permissions.Models;

namespace UserManagement.Api.Infrastructure.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
}
