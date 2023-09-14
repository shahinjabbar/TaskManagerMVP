namespace TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;

public class UserTask
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Deadline { get; set; }
    public UserTaskStatus Status { get; set; } 
    public int OrganizationId { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

public enum UserTaskStatus
{
    ToDo = 0,
    InProgress = 1,
    Done = 2,
}