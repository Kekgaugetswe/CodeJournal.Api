using CodeJournal.Api.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.AccountManagement.Services;

/// <summary>
/// Background service that removes unconfirmed user accounts older than 48 hours every 6 hours.
/// </summary>
public sealed class UnconfirmedUserCleanupService(
    IServiceProvider serviceProvider,
    ILogger<UnconfirmedUserCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);
    private static readonly TimeSpan MaxUnconfirmedAge = TimeSpan.FromHours(48);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during unconfirmed user cleanup");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var cutoff = DateTime.UtcNow.Subtract(MaxUnconfirmedAge);

        var staleUsers = await db.Users
            .Where(u => !u.EmailConfirmed && u.CreatedAt < cutoff)
            .ToListAsync(ct);

        if (staleUsers.Count == 0)
            return;

        db.Users.RemoveRange(staleUsers);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Removed {Count} unconfirmed user(s) older than 48 hours", staleUsers.Count);
    }
}
