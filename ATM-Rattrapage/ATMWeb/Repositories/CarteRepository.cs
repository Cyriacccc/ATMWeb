
using ATMWeb.Data;

// Import des classes métier (CarteBancaire, Compte, Operation)
using ATMWeb.Model;

// Import d’Entity Framework pour utiliser Include / ThenInclude
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Repositories;

// CarteRepository implémente ICarteRepository
// Il reçoit le DataContext via injection de dépendances
public class CarteRepository(DataContext context) : ICarteRepository
{
    // Méthode qui permet de récupérer une carte bancaire à partir de son numéro
    public CarteBancaire? GetByNumeroCarte(string numeroCarte)
    {
        return context
            .CartesBancaires

            // On charge le compte associé à la carte
            .Include(c => c.Compte)

                // On charge aussi les opérations du compte
                .ThenInclude(c => c.Operations)

            // On cherche la carte correspondant au numéro donné
            // Si aucune carte n’est trouvée → retourne null
            .FirstOrDefault(c => c.NumeroCarte == numeroCarte);
    }

    // Méthode pour indiquer qu’une carte a été modifiée
    public void Update(CarteBancaire carte)
    {
        // On dit à Entity Framework que l’objet a changé
        context.CartesBancaires.Update(carte);
    }

    // Méthode pour sauvegarder les modifications en base
    public void SaveChanges()
    {
        // Enregistre toutes les modifications (Update, Add, etc.)
        context.SaveChanges();
    }
}