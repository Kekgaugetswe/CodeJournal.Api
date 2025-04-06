using System;
using CodeJournal.Api.Domain.BlogPosts.Models;
using CodeJournal.Api.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }

}
