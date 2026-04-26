// Import des classes métier (Compte, Operation)
using ATMWeb.Model;

// Import de l’interface repository
using ATMWeb.Repositories;

namespace ATMWeb.UnitTests.Repositories;

// Fake repository utilisé pour les tests
// Il remplace la base de données par des objets en mémoire
public class FakeCompteRepository : ICompteRepository
{
    // Simulation d’un compte stocké en mémoire
    public Compte? Compte { get; set; }

    // Liste en mémoire pour simuler les opérations (retraits / versements)
    public List<Operation> Operations { get; } = [];

    // Simule la récupération d’un compte par son ID
    public Compte? GetById(int id)
    {
        // Si le compte existe et correspond à l'id → on le retourne
        return Compte?.Id == id ? Compte : null;
    }

    // Simule la mise à jour du compte
    public void Update(Compte compte)
    {
        // On remplace simplement l’objet en mémoire
        Compte = compte;
    }

    // Simule l’ajout d’une opération
    public void AddOperation(Operation operation)
    {
        // On ajoute dans la liste en mémoire
        Operations.Add(operation);
    }

    // Permet de supprimer des opérations (utile pour certains tests)
    public void RemoveOperations(IEnumerable<Operation> operations)
    {
        foreach (var operation in operations.ToList())
        {
            Operations.Remove(operation);
        }
    }

    // Simulation de la sauvegarde (inutile ici car pas de base)
    public void SaveChanges() { }
}