using Apollo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Apollo.Data.Repository;

public class ApolloDbContext : DbContext
{
    public ApolloDbContext(DbContextOptions<ApolloDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Research> Research { get; set; }
}
