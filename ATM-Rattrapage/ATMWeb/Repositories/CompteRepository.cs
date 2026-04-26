// Import du DataContext, qui permet d'accéder à la base de données
using ATMWeb.Data;

// Import des classes métier : Compte et Operation
using ATMWeb.Model;

// Import d'Entity Framework Core pour utiliser Include
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Repositories;

// CompteRepository implémente ICompteRepository.
// Il reçoit le DataContext par injection de dépendances.
public class CompteRepository(DataContext context) : ICompteRepository
{
    // Récupère un compte à partir de son identifiant
    public Compte? GetById(int id)
    {
        return context
            .Comptes

            // Charge la carte bancaire associée au compte
            .Include(c => c.CarteBancaire)

            // Charge aussi les opérations liées au compte
            .Include(c => c.Operations)

            // Cherche le compte correspondant à l'id donné
            // Si aucun compte n'est trouvé, retourne null
            .FirstOrDefault(c => c.Id == id);
    }

    // Indique à Entity Framework que le compte a été modifié
    public void Update(Compte compte)
    {
        context.Comptes.Update(compte);
    }

    // Ajoute une nouvelle opération en base
    // Exemple : retrait ou versement
    public void AddOperation(Operation operation)
    {
        context.Operations.Add(operation);
    }

    // Sauvegarde réellement les modifications dans la base SQLite
    public void SaveChanges()
    {
        context.SaveChanges();
    }
}