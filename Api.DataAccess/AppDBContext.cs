using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Rektor> Rektors { get; set; } = null!;
        public DbSet<Grades> Grades { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.email)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.username)
                .IsUnique();

            modelBuilder.Entity<Grades>(entity =>
            {
                entity.HasOne(g => g.teacher)
                    .WithMany(u => u.CreatedGrades)
                    .HasForeignKey(g => g.teacher_id);

                entity.HasOne(g => g.prorektor)
                    .WithMany(u => u.DecidedGrades)
                    .HasForeignKey(g => g.prorektor_id);

                entity.HasOne(g => g.rektor)
                    .WithMany(r => r.Grades)
                    .HasForeignKey(g => g.rektor_id);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
