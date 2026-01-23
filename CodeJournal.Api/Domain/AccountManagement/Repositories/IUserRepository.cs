using System;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace CodeJournal.Api.Domain.AccountManagement.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    
    Task<bool> Add(ApplicationUser user, string password, List<string> roles);
    Task Delete(Guid userId);
}
