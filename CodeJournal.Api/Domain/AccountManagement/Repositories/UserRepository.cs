using System;
using System.Runtime.CompilerServices;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.AccountManagement.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext authDbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(AuthDbContext authDbContext, UserManager<ApplicationUser> userManager)
    {
        this.authDbContext = authDbContext;
        _userManager = userManager;
    }
    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        var users = await authDbContext.Users.ToListAsync();

        var AdminUser = await authDbContext.Users.FirstOrDefaultAsync(x => x.Email == "admin@codejournalx.com");
        if (AdminUser != null)
        {
            users.Remove(AdminUser);
        }

        return users;
    }

    public async Task<bool> Add(ApplicationUser user, string password, List<string> roles)
    {
       var identityResults= await _userManager.CreateAsync(user, password);
       if (identityResults.Succeeded)
       {
          identityResults= await _userManager.AddToRolesAsync(user, roles);

          if (identityResults.Succeeded)
          {
              return true;
          }
       }
       return false;    
       
    }



    public async Task Delete(Guid userId)
    {
        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        
    }
}
