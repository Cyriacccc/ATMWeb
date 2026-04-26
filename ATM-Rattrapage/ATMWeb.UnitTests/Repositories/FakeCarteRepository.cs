// Import des classes métier (CarteBancaire)
using ATMWeb.Model;

// Import de l’interface du repository réel
using ATMWeb.Repositories;

namespace ATMWeb.UnitTests.Repositories;

// Fake repository utilisé uniquement pour les tests
// Il remplace le vrai repository (qui utilise la base de données)
public class FakeCarteRepository : ICarteRepository
{
    // Stockage en mémoire d’une carte (simulation de la base)
    public CarteBancaire? Carte { get; set; }

    // Simulation de la récupération d’une carte par numéro
    public CarteBancaire? GetByNumeroCarte(string numeroCarte)
    {
        // Si aucune carte n’est définie → retourne null
        if (Carte is null)
        {
            return null;
        }

        // Si le numéro correspond → retourne la carte
        return Carte.NumeroCarte == numeroCarte ? Carte : null;
    }

    // Simulation de la mise à jour de la carte
    public void Update(CarteBancaire carte)
    {
        // On remplace simplement la carte en mémoire
        Carte = carte;
    }

    // Simulation de la sauvegarde (inutile ici)
    public void SaveChanges() { }
}