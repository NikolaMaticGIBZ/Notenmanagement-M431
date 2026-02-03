using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess;

/// <summary>
/// Adds Context to MySQL Database
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
public class AppDBContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDBContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Rektor> Rektor { get; set; } = null!;
    public DbSet<Grade> Grade { get; set; } = null!;
    public DbSet<GradeLedgerEntry> GradeLedger { get; set; } = null!;

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
    /// define extension methods on this object that allow you to configure aspects of the model that are specific
    /// to a given database.</param>
    /// <remarks>
    /// <para>
    /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    /// then this method will not be run. However, it will still run when creating a compiled model.
    /// </para>
    /// <para>
    /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    /// examples.
    /// </para>
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasOne(g => g.Teacher)
                .WithMany(u => u.CreatedGrades)
                .HasForeignKey(g => g.Teacher_id);

            entity.HasOne(g => g.Prorektor)
                .WithMany(u => u.DecidedGrades)
                .HasForeignKey(g => g.prorektor_id);

            entity.HasOne(g => g.Rektor)
                .WithMany(r => r.Grades)
                .HasForeignKey(g => g.Rektor_id);
        });


        base.OnModelCreating(modelBuilder);
    }
}
