using System;
using CodeJournal.Api.Domain.BlogPosts.Models;
using CodeJournal.Api.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogImage> BlogImages { get; set; }
    public DbSet<BlogPostLike> BlogPostLike { get; set; }
    public DbSet<BlogPostComment> BlogPostComment { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BlogPostLike>()
                .HasIndex(l => new { l.BlogPostId, l.UserId })
                .IsUnique();

         modelBuilder.Entity<BlogPostLike>()
                .HasOne(l => l.BlogPost)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

        // Comment reply self-referencing relationship
        modelBuilder.Entity<BlogPostComment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

        // Comment likes
        modelBuilder.Entity<CommentLike>()
                .HasIndex(x => new { x.CommentId, x.UserId })
                .IsUnique();

        modelBuilder.Entity<CommentLike>()
                .HasOne(x => x.Comment)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
    }

    
}
