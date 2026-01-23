using Microsoft.AspNetCore.Identity;

namespace CodeJournal.Api.Domain.AccountManagement.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}