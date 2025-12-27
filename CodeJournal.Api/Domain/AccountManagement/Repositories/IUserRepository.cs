using System;
using Microsoft.AspNetCore.Identity;

namespace CodeJournal.Api.Domain.AccountManagement.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<IdentityUser>> GetAllAsync();
    
    Task<bool> Add(IdentityUser user, string password, List<string> roles);
    Task Delete(Guid userId);
}
