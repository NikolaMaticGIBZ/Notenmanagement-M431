using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
    {
    }
    public DbSet<Users> Users { get; set; }
    public DbSet<Rektor> Rektors { get; set; }
    public DbSet<Grades> Grades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>()
            .HasIndex(u => u.email)
            .IsUnique();

        modelBuilder.Entity<Users>()
            .HasIndex(u => u.username)
            .IsUnique();
    }
}
