using Microsoft.EntityFrameworkCore;
using BlogDesk.Api.Models;

namespace BlogDesk.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PostView> PostViews => Set<PostView>();
    public DbSet<PostRating> PostRatings => Set<PostRating>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<AdminOtp> AdminOtps => Set<AdminOtp>();
    public DbSet<AdminSession> AdminSessions => Set<AdminSession>();
    public DbSet<AdminAction> AdminActions => Set<AdminAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One rating per (post, email) -- re-rating updates instead of duplicating.
        modelBuilder.Entity<PostRating>()
            .HasIndex(r => new { r.PostSlug, r.ReaderEmail })
            .IsUnique();

        // Fast "how many views does this slug have" queries.
        modelBuilder.Entity<PostView>()
            .HasIndex(v => v.PostSlug);

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.PostSlug);
    }
}