using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using BCrypt.Net;

namespace OrderManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Cada DbSet representa uma tabela no banco
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seed — usuário admin padrão
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 999,
            Name = "Administrador",
            Email = "admin@orderapi.com",
            PasswordHash = "$2a$11$9Hy7V3K2vJnGQ7z5Z8yX8OqW1mN4pL6rT0uI2eA3bC5dF7gH9jK1m",
            Role = "Admin",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }

    // Atualiza UpdatedAt automaticamente ao salvar
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    } 
}