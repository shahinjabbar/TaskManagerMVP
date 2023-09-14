using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;

namespace TaskManager.Infrastructure.EntityFrameworkDataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<UserTask> UserTasks { get; set; }
}
