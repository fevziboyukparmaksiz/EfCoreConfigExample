using EfCoreConfigExample.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCoreConfigExample;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(builder =>
        {
            builder.ToTable("Companies");

            builder
                .HasMany(company => company.Employees)
                .WithOne()
                .HasForeignKey(employee => employee.CompanyId)
                .IsRequired();
        });

        modelBuilder.Entity<Employee>(builder => builder.ToTable("Employees"));
    }
}
