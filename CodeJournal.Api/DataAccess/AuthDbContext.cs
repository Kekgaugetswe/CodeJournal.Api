using System.Collections.Generic;
using CodeJournal.Api.Domain.AccountManagement.Enums;
using CodeJournal.Api.Domain.AccountManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.DataAccess;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ---- ApplicationUser mappings (PostgreSQL friendly) ----
        builder.Entity<ApplicationUser>(b =>
        {
            // Store enum as int in DB (portable across providers)
            b.Property(x => x.Status)
                .HasConversion<int>();

            // Defaults for new rows
            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            b.Property(x => x.IsBlocked)
                .HasDefaultValue(false);

            b.Property(x => x.WarningCount)
                .HasDefaultValue(0);

            // NOTE:
            // Do NOT set HasDefaultValue(AccountStatus.Active) here unless you also configure a sentinel
            // (otherwise EF warns because enum default 0 triggers DB default always).
            // We’ll set Status explicitly in code when creating users.
        });

        // ---- RefreshToken entity configuration ----
        builder.Entity<RefreshToken>(b =>
        {
            b.HasKey(rt => rt.Id);

            b.Property(rt => rt.Id)
                .ValueGeneratedOnAdd();

            b.Property(rt => rt.UserId)
                .IsRequired()
                .HasMaxLength(450);

            b.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(rt => rt.ExpiresAt)
                .IsRequired();

            b.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            b.Property(rt => rt.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            b.HasIndex(rt => rt.TokenHash);

            b.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Seed Roles (deterministic: safe for migrations) ----
        var readerRoleId = "3ef9235c-df3d-4d09-a54e-03adc9ed2283";
        var writerRoleId = "12a1d508-95f2-4fe2-a712-532fca8e5b9f";

        var roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = readerRoleId,
                Name = "Reader",
                NormalizedName = "READER",
                ConcurrencyStamp = readerRoleId
            },
            new IdentityRole
            {
                Id = writerRoleId,
                Name = "Writer",
                NormalizedName = "WRITER",
                ConcurrencyStamp = writerRoleId
            }
        };

        builder.Entity<IdentityRole>().HasData(roles);
        
    }
}