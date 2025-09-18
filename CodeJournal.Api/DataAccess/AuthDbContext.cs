using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.DataAccess;

public class AuthDbContext : IdentityDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Create Reader and Writer roles

        var readerRoleId = "3ef9235c-df3d-4d09-a54e-03adc9ed2283";
        var writerRoleId = "12a1d508-95f2-4fe2-a712-532fca8e5b9f";

        var roles = new List<IdentityRole>
        {
            new IdentityRole(){
                Id = readerRoleId,
                Name = "Reader",
                NormalizedName = "Reader".ToUpper(),
                ConcurrencyStamp = readerRoleId

            },
            new IdentityRole(){
                Id = writerRoleId,
                Name = "Writer",
                NormalizedName = "Writer".ToUpper(),
                ConcurrencyStamp = writerRoleId }
        };


        //seed the roles
        builder.Entity<IdentityRole>().HasData(roles);

        // Create a default admin user
        var adminUserId = "dbbe523f-8929-44e0-b440-32ebd86f526d";

        var admin = new IdentityUser()
        {
            Id = adminUserId,
            UserName = "admin@codejournalx.com",
            NormalizedUserName = "admin@codejournalx.com".ToUpper(),
            Email = "admin@codejournalx.com",
            NormalizedEmail = "admin@codejournalx.com".ToUpper(),

        };
        admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

        builder.Entity<IdentityUser>().HasData(admin);

        //give Roles to the admin user
        var adminRoles = new List<IdentityUserRole<string>>()
        {
            new IdentityUserRole<string>()
            {
                UserId = adminUserId,
                RoleId = readerRoleId
            },
            new IdentityUserRole<string>()
            {
                UserId = adminUserId,
                RoleId = writerRoleId
            }
        };
        
        builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
    }
}
 