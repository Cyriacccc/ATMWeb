// Import de la classe métier CarteBancaire
using ATMWeb.Model;

namespace ATMWeb.Repositories;

// Interface du repository des cartes bancaires
// Elle définit ce que le repository DOIT faire (sans dire comment)
public interface ICarteRepository
{
    // Permet de récupérer une carte à partir de son numéro
    // Peut retourner null si la carte n’existe pas
    CarteBancaire? GetByNumeroCarte(string numeroCarte);

    // Permet de modifier une carte existante (ex : bloquer la carte, changer le PIN)
    void Update(CarteBancaire carte);

    // Permet de sauvegarder les modifications en base de données
    void SaveChanges();
}