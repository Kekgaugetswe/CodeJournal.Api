using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CodeJournal.Api.Domain.AccountManagement.Respositories;

public class TokenRepository(IConfiguration configuration, AuthDbContext authDbContext) : ITokenRepository
{
    public string CreateAccessToken(IdentityUser user, List<string> roles)
    {
        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)

        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        // JWT security Token Parameters

        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        var expiryMinutes = configuration.GetValue<int>("JWT:AccessTokenExpiryMinutes", 120);

         var token  = new JwtSecurityToken(
            issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: credentials
         );

         // return token

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> CreateRefreshTokenAsync(string userId)
    {
        // Generate 32 bytes of cryptographically random data
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var rawToken = Convert.ToBase64String(randomBytes);

        // Compute SHA-256 hash of the raw token for database storage
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        var tokenHash = Convert.ToBase64String(hashBytes);

        // Read expiry from configuration (default 7 days)
        var expiryDays = configuration.GetValue<int>("JWT:RefreshTokenExpiryDays", 7);

        // Create and save RefreshToken entity
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await authDbContext.RefreshTokens.AddAsync(refreshToken);
        await authDbContext.SaveChangesAsync();

        // Return the raw (unhashed) token to the caller
        return rawToken;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        // Compute SHA-256 hash of the incoming raw token
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToBase64String(hashBytes);

        // Query for a matching, non-revoked, non-expired token
        return await authDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash
                                       && !rt.IsRevoked
                                       && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeRefreshTokenAsync(string tokenHash)
    {
        var refreshToken = await authDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (refreshToken is null)
            return;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;

        await authDbContext.SaveChangesAsync();
    }
}
