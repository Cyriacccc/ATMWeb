using ATMWeb.Model;

namespace ATMWeb.Data;

// Classe statique → utilisée uniquement pour initialiser la base
public static class SeedData
{
    // Méthode appelée au démarrage dans Program.cs
    public static void Initialize(DataContext context)
    {
        // Si des données existent déjà → on ne fait rien
        if (context.Comptes.Any() || context.CartesBancaires.Any())
        {
            return;
        }

        // Création d’un compte avec un solde initial de 100
        var compte = new Compte { Solde = 100.0m };

        // Création d’une carte liée à ce compte
        var carte = new CarteBancaire
        {
            NumeroCarte = "123456",           // numéro utilisé dans les tests
            Pin = "0000",                     // code PIN par défaut
            EstBloquee = false,               // carte active
            NombreEssaisRestants = 3,         // 3 essais autorisés
            Compte = compte,                  // lien avec le compte
        };

        // Ajout en base
        context.Comptes.Add(compte);
        context.CartesBancaires.Add(carte);

        // Sauvegarde
        context.SaveChanges();
    }
}