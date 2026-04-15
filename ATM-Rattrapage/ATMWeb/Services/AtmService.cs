using ATMWeb.Exceptions;
using ATMWeb.Model;
using ATMWeb.Repositories;

namespace ATMWeb.Services;

public class AtmService(
    ICarteRepository carteRepository,
    ICompteRepository compteRepository
) : IAtmService
{
    public decimal ConsulterSolde(string numeroCarte, string pin)
    {
        var carte = AuthentifierCarte(numeroCarte, pin);
        return carte.Compte.Solde;
    }

    public decimal EffectuerVersement(string numeroCarte, string pin, decimal montant)
    {
        if (montant <= 0)
        {
            throw new DomainValidationException("Le montant doit être positif");
        }

        var carte = AuthentifierCarte(numeroCarte, pin);
        var compte = carte.Compte;

        compte.Solde += montant;

        compteRepository.Update(compte);
        compteRepository.AddOperation(
            new Operation
            {
                Type = "Versement",
                Montant = montant,
                DateOperation = DateTime.UtcNow,
                CompteId = compte.Id
            }
        );
        compteRepository.SaveChanges();

        return compte.Solde;
    }

    public decimal EffectuerRetrait(string numeroCarte, string pin, decimal montant)
    {
        if (montant <= 0)
        {
            throw new DomainValidationException("Le montant doit être positif");
        }

        var carte = AuthentifierCarte(numeroCarte, pin);
        var compte = carte.Compte;

        if (montant > compte.Solde)
        {
            throw new DomainValidationException("Solde insuffisant");
        }

        compte.Solde -= montant;

        compteRepository.Update(compte);
        compteRepository.AddOperation(
            new Operation
            {
                Type = "Retrait",
                Montant = montant,
                DateOperation = DateTime.UtcNow,
                CompteId = compte.Id
            }
        );
        compteRepository.SaveChanges();

        return compte.Solde;
    }

    public void Reset()
    {
        var carte = carteRepository.GetByNumeroCarte("123456");

        if (carte is null)
        {
            throw new NotFoundException("Carte de test introuvable");
        }

        var compte = carte.Compte;

        compte.Solde = 100.0m;
        carte.Pin = "0000";
        carte.EstBloquee = false;
        carte.NombreEssaisRestants = 3;

        compte.Operations.Clear();

        compteRepository.Update(compte);
        carteRepository.Update(carte);
        compteRepository.SaveChanges();
    }

    private CarteBancaire AuthentifierCarte(string numeroCarte, string pin)
    {
        var carte = carteRepository.GetByNumeroCarte(numeroCarte);

        if (carte is null)
        {
            throw new NotFoundException("Carte introuvable");
        }

        if (carte.EstBloquee)
        {
            throw new ForbiddenException("Action impossible : carte bloquée");
        }

        if (carte.Pin != pin)
        {
            carte.NombreEssaisRestants--;

            if (carte.NombreEssaisRestants <= 0)
            {
                carte.NombreEssaisRestants = 0;
                carte.EstBloquee = true;
                carteRepository.Update(carte);
                carteRepository.SaveChanges();

                throw new ForbiddenException("Carte bloquée");
            }

            carteRepository.Update(carte);
            carteRepository.SaveChanges();

            var motEssai = carte.NombreEssaisRestants == 1 ? "essai restant" : "essais restants";
            throw new UnauthorizedException(
                $"PIN incorrect. {carte.NombreEssaisRestants} {motEssai}"
            );
        }

        if (carte.NombreEssaisRestants != 3)
        {
            carte.NombreEssaisRestants = 3;
            carteRepository.Update(carte);
            carteRepository.SaveChanges();
        }

        return carte;
    }
}