using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Infrastructure.EntityFrameworkDataAccess.Entities;
using TaskManager.Infrastructure.EntityFrameworkDataAccess;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public OrganizationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // also there could be some changes like moving logic to seperate place(application layer). I just went straight forward :)
    [HttpPost]
    public async Task<IActionResult> Post(OrganizationSignUpModel model)
    {
        var organization = new Organization
        {
            Name = model.OrganizationName,
            PhoneNumber = model.PhoneNumber,
            Address = model.Address
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        var adminUser = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.AdminEmail,
            PhoneNumber = model.PhoneNumber,
            OrganizationId = organization.Id
        };

        var result = await _userManager.CreateAsync(adminUser, model.AdminPassword);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await _userManager.AddToRoleAsync(adminUser, "Admin");

            return Ok();
        }
        return BadRequest(result.Errors);
    }
}

public record OrganizationSignUpModel(string OrganizationName, string PhoneNumber, string Address, string UserName, string AdminEmail, string AdminPassword);