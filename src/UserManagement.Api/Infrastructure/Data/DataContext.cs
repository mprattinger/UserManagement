using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Features.Auth.Models;

namespace UserManagement.Api.Infrastructure.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
}
