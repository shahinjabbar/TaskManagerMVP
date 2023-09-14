namespace TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

}