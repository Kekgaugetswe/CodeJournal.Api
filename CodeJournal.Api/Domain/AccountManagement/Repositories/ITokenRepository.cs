using Microsoft.AspNetCore.Identity;
using CodeJournal.Api.Domain.AccountManagement.Models;

namespace CodeJournal.Api.Domain.AccountManagement.Respositories;

public interface ITokenRepository
{
    string CreateAccessToken(IdentityUser user, List<string> roles);
    Task<string> CreateRefreshTokenAsync(string userId);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string tokenHash);
}
