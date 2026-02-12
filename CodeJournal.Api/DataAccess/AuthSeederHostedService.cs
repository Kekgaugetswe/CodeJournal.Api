using CodeJournal.Api.Domain.AccountManagement.Enums;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.DataAccess;

public sealed class AuthSeederHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public AuthSeederHostedService(IServiceProvider sp, IWebHostEnvironment env, IConfiguration config)
    {
        _sp = sp;
        _env = env;
        _config = config;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // gate it (recommended)
        var seedEnabled = _config.GetValue<bool>("SeedAuth");
        if (!seedEnabled) return;

        // optional: only in development
        // if (!_env.IsDevelopment()) return;

        using var scope = _sp.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Reader", "Writer" };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        const string adminEmail = "admin@codejournalx.com";
        const string adminPassword = "Admin@123";

        var admin = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail, cancellationToken);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Admin",
                DisplayName = "Admin",
                Status = AccountStatus.Active,
                IsBlocked = false,
                WarningCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
                throw new Exception("Failed to create admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        foreach (var role in roles)
            if (!await userManager.IsInRoleAsync(admin, role))
                await userManager.AddToRoleAsync(admin, role);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}