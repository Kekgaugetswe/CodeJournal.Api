using CodeJournal.Api.Domain.AccountManagement.Enums;
using Microsoft.AspNetCore.Identity;

namespace CodeJournal.Api.Domain.AccountManagement.Models;

public class ApplicationUser : IdentityUser
{
    // Profile
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }

    // Status / moderation
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
    public string? BlockReason { get; set; }
    public int WarningCount { get; set; }
    public string? ModerationNotes { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public DateTime? LastPasswordChangeAt { get; set; }

    // Preferences
    public string? TimeZone { get; set; }
    public string? Locale { get; set; }
    public bool MarketingOptIn { get; set; }

    // SaaS (optional)
    public Guid? TenantId { get; set; }

    // Soft delete (optional)
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedReason { get; set; }
}