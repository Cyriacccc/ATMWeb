using ATMWeb.Model;

namespace ATMWeb.Data;

public static class SeedData
{
    public static void Initialize(DataContext context)
    {
        if (context.Comptes.Any() || context.CartesBancaires.Any())
        {
            return;
        }

        var compte = new Compte
        {
            Solde = 100.0m
        };

        var carte = new CarteBancaire
        {
            NumeroCarte = "123456",
            Pin = "0000",
            EstBloquee = false,
            NombreEssaisRestants = 3,
            Compte = compte
        };

        context.Comptes.Add(compte);
        context.CartesBancaires.Add(carte);
        context.SaveChanges();
    }
}