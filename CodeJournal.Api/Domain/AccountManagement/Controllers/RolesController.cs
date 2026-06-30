using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.AccountManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "SuperAdmin")]
public class RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    : ControllerBase
{
    // CREATE ROLE
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var roleName = dto.RoleName?.Trim();
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("RoleName is required.");

        if (await roleManager.RoleExistsAsync(roleName))
            return Conflict($"Role '{roleName}' already exists.");
        
        var role = new IdentityRole(roleName)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await roleManager.CreateAsync(role);
        return result.Succeeded ? Ok(new { roleName }) : BadRequest(result.Errors);
    }

    //  GET ALL ROLES
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();

        return Ok(roles);
    }

    // GET ROLE BY ID
    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetById(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null) return NotFound();

        return Ok(new { role.Id, role.Name });
    }

    // DELETE ROLE (guard against deleting core roles if you want)
    [HttpDelete("{roleId}")]
    public async Task<IActionResult> Delete(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null) return NotFound();

        if (role.Name == "SuperAdmin")
            return BadRequest("You cannot delete the SuperAdmin role.");

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    // ASSIGN ROLE TO USER
    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignRoleDto dto)
    {
        var roleName = dto.RoleName?.Trim();
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("RoleName is required.");

        var user = await FindUser(dto.UserIdOrEmail);
        if (user is null) return NotFound("User not found.");

        if (!await roleManager.RoleExistsAsync(roleName))
            return NotFound($"Role '{roleName}' does not exist.");

        if (await userManager.IsInRoleAsync(user, roleName))
            return Ok(new { message = "User already has this role." });

        var result = await userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded ? Ok(new { userId = user.Id, roleName }) : BadRequest(result.Errors);
    }

    //  REMOVE ROLE FROM USER
    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] AssignRoleDto dto)
    {
        var roleName = dto.RoleName?.Trim();
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("RoleName is required.");

        var user = await FindUser(dto.UserIdOrEmail);
        if (user is null) return NotFound("User not found.");

        if (!await roleManager.RoleExistsAsync(roleName))
            return NotFound($"Role '{roleName}' does not exist.");

        if (!await userManager.IsInRoleAsync(user, roleName))
            return Ok(new { message = "User does not have this role." });

        // Optional safety: don't allow removing SuperAdmin from yourself
        var currentUserId = userManager.GetUserId(User);
        if (roleName == "SuperAdmin" && currentUserId == user.Id)
            return BadRequest("You cannot remove SuperAdmin from yourself.");

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded ? Ok(new { userId = user.Id, roleName }) : BadRequest(result.Errors);
    }

    //  GET USER ROLES
    [HttpGet("user/{userIdOrEmail}")]
    public async Task<IActionResult> GetUserRoles(string userIdOrEmail)
    {
        var user = await FindUser(userIdOrEmail);
        if (user is null) return NotFound("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, roles });
    }

    private async Task<ApplicationUser?> FindUser(string userIdOrEmail)
    {
        var value = userIdOrEmail?.Trim();
        if (string.IsNullOrWhiteSpace(value)) return null;

        return value.Contains("@")
            ? await userManager.FindByEmailAsync(value)
            : await userManager.FindByIdAsync(value);
    }
    
}