using System;
using Microsoft.AspNetCore.Identity;

namespace CodeJournal.Api.Domain.AccountManagement.Respositories;

public interface ITokenRepository
{

    string CreateToken(IdentityUser user, List<string> roles);

}
