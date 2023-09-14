using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;
using TaskManager.Infrastructure.EntityFrameworkDataAccess;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context=context;
        _roleManager=roleManager;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post(CreateUserModel model)
    {
        var claimsIdentity = this.User.Identity as ClaimsIdentity;
        var organizationIdClaim = claimsIdentity!.FindFirst("OrganizationId");
        if (organizationIdClaim == null)
        {
            return Unauthorized();
        }
        int organizationId = int.Parse(organizationIdClaim.Value);

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            OrganizationId = organizationId,
            Surname = model.Surname
        };

        var result = await _userManager.CreateAsync(user, model.DefaultPassword);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpPost("tasks")]
    public async Task<IActionResult> Post([FromBody] CreateTaskModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var newTask = new UserTask
        {
            Title = model.Title,
            Description = model.Description,
            Deadline = model.Deadline,
            Status = model.Status,
            UserId =  string.IsNullOrEmpty(model.AssignedUserId) ? currentUser!.Id : model.AssignedUserId,
            OrganizationId = currentUser!.OrganizationId
        };

        _context.UserTasks.Add(newTask);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Task created successfully!" });
    }

    [Authorize]
    [HttpGet("tasks")]
    public async Task<IActionResult> GetTasks()
    {
        var claimsIdentity = this.User.Identity as ClaimsIdentity;
        var organizationIdClaim = claimsIdentity!.FindFirst("OrganizationId");
        if (organizationIdClaim == null)
        {
            return Unauthorized();
        }
        int organizationId = int.Parse(organizationIdClaim.Value);

        var tasks = await _context.UserTasks
                            .Where(t => t.OrganizationId == organizationId)
                            .ToListAsync();

        return Ok(tasks);
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> Get()
    {
        var claimsIdentity = this.User.Identity as ClaimsIdentity;
        var organizationIdClaim = claimsIdentity!.FindFirst("OrganizationId");
        if (organizationIdClaim == null)
        {
            return Unauthorized();
        }
        int organizationId = int.Parse(organizationIdClaim.Value);

        var tasks = await _context.Users
                            .Where(t => t.OrganizationId == organizationId)
                            .ToListAsync();

        return Ok(tasks);
    }
}

public record CreateUserModel(string Username, string Surname, string Email, string DefaultPassword);

public record CreateTaskModel(string Title, string Description, DateTime Deadline, UserTaskStatus Status, string AssignedUserId);

