// On importe les classes métier : Compte, CarteBancaire, Operation
using ATMWeb.Model;

// On importe Entity Framework Core
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Data;

// DataContext hérite de DbContext.
// C’est la classe centrale utilisée par Entity Framework pour communiquer avec la base.
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    // Représente la table des comptes dans la base de données
    public DbSet<Compte> Comptes => Set<Compte>();

    // Représente la table des cartes bancaires
    public DbSet<CarteBancaire> CartesBancaires => Set<CarteBancaire>();

    // Représente la table des opérations
    public DbSet<Operation> Operations => Set<Operation>();

    // Cette méthode permet de configurer les relations entre les tables
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ici on configure la relation entre Compte et CarteBancaire
        // Un compte possède une seule carte bancaire
        modelBuilder
            .Entity<Compte>()
            .HasOne(c => c.CarteBancaire)

            // Une carte bancaire est liée à un seul compte
            .WithOne(cb => cb.Compte)

            // La clé étrangère est CompteId dans CarteBancaire
            .HasForeignKey<CarteBancaire>(cb => cb.CompteId);

        // On appelle la configuration de base d’Entity Framework
        base.OnModelCreating(modelBuilder);
    }
}