using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CodeJournal.Api.Domain.AccountManagement.Respositories;

public class TokenRepository( IConfiguration configuration) : ITokenRepository
{
    public string CreateToken(IdentityUser user, List<string> roles)
    {
        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email)

        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        // JWT security Token Parameters

        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
         
         var token  = new JwtSecurityToken(
            issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
         );

         // return token

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
