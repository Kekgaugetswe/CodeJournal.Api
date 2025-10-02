using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Respositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.AccountManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository) : ControllerBase
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

                var jwtToken = tokenRepository.CreateToken(identityUser, roles.ToList());

                var response = new LoginResponseDto()
                {
                    Email = request.Email,
                    Roles = roles.ToList(),
                    Token = jwtToken,
                    UserId = identityUser.Id
                };

                return Ok(response);
            }


        }
        ModelState.AddModelError("", "Email or Password is incorrect");

        return ValidationProblem(ModelState);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        //Create IdentityUser Object

        var user = new IdentityUser()
        {
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
