using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<MonthlyBudget> MonthlyBudgets => Set<MonthlyBudget>();
    public DbSet<CryptoAsset> CryptoAssets => Set<CryptoAsset>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        // User
        model.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        // Category
        model.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.HasOne(c => c.User).WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Expense
        model.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasOne(x => x.Category).WithMany(c => c.Expenses)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.User).WithMany(u => u.Expenses)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // MonthlyBudget — one budget per category/month/year/user
        model.Entity<MonthlyBudget>(e =>
        {
            e.HasKey(b => b.Id);
            e.HasIndex(b => new { b.UserId, b.CategoryId, b.Month, b.Year }).IsUnique();
            e.Property(b => b.BudgetAmount).HasColumnType("decimal(18,2)");
            e.HasOne(b => b.Category).WithMany(c => c.MonthlyBudgets)
                .HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(b => b.User).WithMany(u => u.MonthlyBudgets)
                .HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // CryptoAsset
        model.Entity<CryptoAsset>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Symbol).HasMaxLength(10).IsRequired();
            e.Property(a => a.Quantity).HasColumnType("decimal(18,8)");
            e.Property(a => a.AveragePurchasePrice).HasColumnType("decimal(18,2)");
            e.HasOne(a => a.User).WithMany(u => u.CryptoAssets)
                .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
