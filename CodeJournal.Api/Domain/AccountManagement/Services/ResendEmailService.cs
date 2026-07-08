using Resend;

namespace CodeJournal.Api.Domain.AccountManagement.Services;

public class ResendEmailService(ResendClient resendClient, IConfiguration configuration, ILogger<ResendEmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var fromAddress = configuration["Email:FromAddress"] ?? "noreply@codejournalx.com";

        var message = new EmailMessage
        {
            From = fromAddress,
            To = [to],
            Subject = subject,
            HtmlBody = htmlBody
        };

        try
        {
            await resendClient.EmailSendAsync(message);
            logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
