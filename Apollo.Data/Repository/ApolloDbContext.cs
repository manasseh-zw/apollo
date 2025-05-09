using Apollo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Apollo.Data.Repository;

public class ApolloDbContext(DbContextOptions<ApolloDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-one relationship between Research and ResearchMindMap
        modelBuilder
            .Entity<Research>()
            .HasOne(r => r.MindMap)
            .WithOne(m => m.Research)
            .HasForeignKey<ResearchMindMap>(m => m.ResearchId);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Research> Research { get; set; }
    public DbSet<ResearchPlan> ResearchPlans { get; set; }
    public DbSet<ResearchReport> ResearchReports { get; set; }
    public DbSet<ResearchMindMap> ResearchMindMaps { get; set; }
}
