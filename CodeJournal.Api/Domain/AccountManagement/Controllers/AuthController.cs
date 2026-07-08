using System.Security.Cryptography;
using System.Text;
using CodeJournal.Api.Domain.AccountManagement.Dtos;
using CodeJournal.Api.Domain.AccountManagement.Models;
using CodeJournal.Api.Domain.AccountManagement.Respositories;
using CodeJournal.Api.Domain.AccountManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CodeJournal.Api.Domain.AccountManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenRepository tokenRepository,
    IEmailService emailService,
    IConfiguration configuration) : ControllerBase
{
    // ──────────────────────────────────────────────
    // POST /api/auth/login
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is not null)
        {
            // Block login if email is not confirmed
            if (!identityUser.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please verify your email address before logging in.");
                return ValidationProblem(ModelState);
            }

            var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.Password);
            if (checkPasswordResult)
            {
                var roles = await userManager.GetRolesAsync(identityUser);

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

    // ──────────────────────────────────────────────
    // POST /api/auth/register
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var email = request.Email.Trim();
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            if (existingUser.EmailConfirmed)
            {
                // Don't reveal that the account exists — return generic validation error
                ModelState.AddModelError("", "Unable to create account. Please try a different email.");
                return ValidationProblem(ModelState);
            }

            // Account exists but unconfirmed — resend verification email
            await SendConfirmationEmailAsync(existingUser);
            return Ok(new { message = "A verification email has been sent. Please check your inbox." });
        }

        var user = new ApplicationUser()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = email,
            Email = email,
            EmailConfirmed = false
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            identityResult = await userManager.AddToRoleAsync(user, "Reader");
            if (identityResult.Succeeded)
            {
                await SendConfirmationEmailAsync(user);
                return Ok(new { message = "Registration successful. Please check your email to verify your account." });
            }
        }

        if (identityResult.Errors.Any())
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return ValidationProblem(ModelState);
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/confirm-email
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(new { message = "Email and token are required." });
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return BadRequest(new { message = "Invalid confirmation request." });
        }

        // Decode from Base64URL back to the original token
        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            return Ok(new { message = "Email confirmed successfully. You can now log in." });
        }

        return BadRequest(new { message = "Email confirmation failed. The link may have expired." });
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/resend-confirmation-email
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email is required." });
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            // Don't reveal whether account exists
            return Ok(new { message = "If an account with that email exists, a verification email has been sent." });
        }

        if (user.EmailConfirmed)
        {
            return Ok(new { message = "Email is already confirmed. You can log in." });
        }

        await SendConfirmationEmailAsync(user);
        return Ok(new { message = "If an account with that email exists, a verification email has been sent." });
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/forgot-password
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        // Always return generic response — never reveal if email exists
        var genericResponse = new { message = "If an account with that email exists, a password reset link has been sent." };

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Ok(genericResponse);
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EmailConfirmed)
        {
            return Ok(genericResponse);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:4200";
        var resetUrl = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

        var htmlBody = $@"
            <h2>Reset Your Password</h2>
            <p>Click the link below to reset your password:</p>
            <a href=""{resetUrl}"">Reset Password</a>
            <p>If you didn't request this, you can safely ignore this email.</p>
            <p>This link will expire in 24 hours.</p>";

        await emailService.SendEmailAsync(user.Email!, "Reset Your Password — CodeJournal", htmlBody);

        return Ok(genericResponse);
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/reset-password
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(new { message = "Email, token, and new password are required." });
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return BadRequest(new { message = "Invalid reset request." });
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (result.Succeeded)
        {
            return Ok(new { message = "Password has been reset successfully. You can now log in." });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return ValidationProblem(ModelState);
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/refresh (existing)
    // ──────────────────────────────────────────────
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest();
        }

        var validToken = await tokenRepository.ValidateRefreshTokenAsync(request.RefreshToken);
        if (validToken is null)
        {
            return Unauthorized();
        }

        await tokenRepository.RevokeRefreshTokenAsync(validToken.TokenHash);

        var user = await userManager.FindByIdAsync(validToken.UserId);
        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);

        var newAccessToken = tokenRepository.CreateAccessToken(user, roles.ToList());
        var newRefreshToken = await tokenRepository.CreateRefreshTokenAsync(user.Id);

        return Ok(new RefreshResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    // ──────────────────────────────────────────────
    // POST /api/auth/revoke (existing)
    // ──────────────────────────────────────────────
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

    // ──────────────────────────────────────────────
    // Private helper: send confirmation email
    // ──────────────────────────────────────────────
    private async Task SendConfirmationEmailAsync(ApplicationUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:4200";
        var confirmUrl = $"{frontendUrl}/confirm-email?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

        var htmlBody = $@"
            <h2>Confirm Your Email</h2>
            <p>Thank you for registering with CodeJournal!</p>
            <p>Click the link below to verify your email address:</p>
            <a href=""{confirmUrl}"">Verify Email</a>
            <p>If you didn't create this account, you can safely ignore this email.</p>";

        await emailService.SendEmailAsync(user.Email!, "Confirm Your Email — CodeJournal", htmlBody);
    }
}
