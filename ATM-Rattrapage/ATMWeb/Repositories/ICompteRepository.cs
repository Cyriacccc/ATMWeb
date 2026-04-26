// Import des classes métier (Compte, Operation)
using ATMWeb.Model;

namespace ATMWeb.Repositories;

// Interface du repository des comptes
// Elle définit les actions possibles sur un compte (sans dire comment)
public interface ICompteRepository
{
    // Permet de récupérer un compte à partir de son identifiant
    // Peut retourner null si le compte n’existe pas
    Compte? GetById(int id);

    // Permet de modifier un compte (ex : changer le solde)
    void Update(Compte compte);

    // Permet d’ajouter une opération (retrait ou versement)
    void AddOperation(Operation operation);

    // Permet de sauvegarder les modifications en base
    void SaveChanges();
}