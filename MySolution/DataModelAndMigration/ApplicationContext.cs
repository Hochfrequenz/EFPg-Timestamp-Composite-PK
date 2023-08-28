namespace DataModelAndMigration;

using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<MyModel> MyModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyModel>().HasKey(m => new { m.GuidPartOfKey, m.DatePartOfKey });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=testdb;Username=testusr;Password=testpwd");
    }
}
