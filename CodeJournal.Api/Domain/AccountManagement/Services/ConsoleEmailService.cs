namespace CodeJournal.Api.Domain.AccountManagement.Services;

/// <summary>
/// Development email service that logs emails to the console.
/// Replace with a real provider (SendGrid, Resend, etc.) for production.
/// </summary>
public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        logger.LogInformation(
            "=== EMAIL ===\nTo: {To}\nSubject: {Subject}\nBody:\n{Body}\n=== END EMAIL ===",
            to, subject, htmlBody);

        return Task.CompletedTask;
    }
}
