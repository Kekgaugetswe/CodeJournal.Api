namespace CodeJournal.Api.Domain.AccountManagement.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
}
