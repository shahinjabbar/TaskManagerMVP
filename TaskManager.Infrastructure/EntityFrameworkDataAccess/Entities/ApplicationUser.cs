using Microsoft.AspNetCore.Identity;

namespace TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();
}
