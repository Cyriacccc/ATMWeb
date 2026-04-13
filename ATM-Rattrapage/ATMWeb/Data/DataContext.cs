using ATMWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Compte> Comptes => Set<Compte>();
    public DbSet<CarteBancaire> CartesBancaires => Set<CarteBancaire>();
    public DbSet<Operation> Operations => Set<Operation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compte>()
            .HasOne(c => c.CarteBancaire)
            .WithOne(cb => cb.Compte)
            .HasForeignKey<CarteBancaire>(cb => cb.CompteId);

        base.OnModelCreating(modelBuilder);
    }
}