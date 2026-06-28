using System.Security.Cryptography;
using System.Text;
using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Models;
using CodeJournal.Api.Domain.AccountManagement.Respositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.AccountManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository) : ControllerBase
{


    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // check if the user exists
        var identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is not null)
        {
            var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.Password);
            if (checkPasswordResult)
            {

                var roles = await userManager.GetRolesAsync(identityUser);

                //Create a Token and response

                var jwtToken = tokenRepository.CreateAccessToken(identityUser, roles.ToList());
                var refreshToken = await tokenRepository.CreateRefreshTokenAsync(identityUser.Id);

                var response = new LoginResponseDto()
                {
                    Email = request.Email,
                    Roles = roles.ToList(),
                    Token = jwtToken,
                    RefreshToken = refreshToken,
                    UserId = identityUser.Id
                };

                return Ok(response);
            }


        }
        ModelState.AddModelError("", "Email or Password is incorrect");

        return ValidationProblem(ModelState);
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        // Validate request body
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest();
        }

        // Validate refresh token against database
        var validToken = await tokenRepository.ValidateRefreshTokenAsync(request.RefreshToken);
        if (validToken is null)
        {
            return Unauthorized();
        }

        // Revoke old token
        await tokenRepository.RevokeRefreshTokenAsync(validToken.TokenHash);

        // Look up user
        var user = await userManager.FindByIdAsync(validToken.UserId);
        if (user is null)
        {
            return Unauthorized();
        }

        // Get roles
        var roles = await userManager.GetRolesAsync(user);

        // Create new access token and refresh token
        var newAccessToken = tokenRepository.CreateAccessToken(user, roles.ToList());
        var newRefreshToken = await tokenRepository.CreateRefreshTokenAsync(user.Id);

        return Ok(new RefreshResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost]
    [Route("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto request)
    {
        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken));
            var tokenHash = Convert.ToBase64String(hashBytes);
            await tokenRepository.RevokeRefreshTokenAsync(tokenHash);
        }

        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        //Create IdentityUser Object

        var user = new ApplicationUser()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
        };
        // Created user
        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            //Assign Reader Role to the user
            identityResult = await userManager.AddToRoleAsync(user, "Reader");
            if (identityResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
        }
        else
        {
            if (identityResult.Errors.Any())
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        return ValidationProblem(ModelState);
    }
}
