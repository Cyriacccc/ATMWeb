// Import des exceptions métier utilisées par le service
using ATMWeb.Exceptions;

// Import des classes métier : Compte, CarteBancaire, Operation
using ATMWeb.Model;

// Import des interfaces de repositories
using ATMWeb.Repositories;

namespace ATMWeb.Services;

// AtmService implémente IAtmService
// Il reçoit deux repositories par injection de dépendances :
// - carteRepository : pour accéder aux cartes bancaires
// - compteRepository : pour modifier les comptes et enregistrer les opérations
public class AtmService(ICarteRepository carteRepository, ICompteRepository compteRepository)
    : IAtmService
{
    // Consulte le solde du compte associé à une carte
    public decimal ConsulterSolde(string numeroCarte, string pin)
    {
        // On authentifie d'abord la carte avec le numéro et le PIN
        var carte = AuthentifierCarte(numeroCarte, pin);

        // Si l'authentification réussit, on retourne le solde du compte associé
        return carte.Compte.Solde;
    }

    // Effectue un versement sur le compte
    public decimal EffectuerVersement(string numeroCarte, string pin, decimal montant)
    {
        // Règle métier : un versement doit être strictement positif
        if (montant <= 0)
        {
            throw new DomainValidationException("Le montant doit être positif");
        }

        // On vérifie que la carte existe, n'est pas bloquée et que le PIN est correct
        var carte = AuthentifierCarte(numeroCarte, pin);

        // On récupère le compte lié à la carte
        var compte = carte.Compte;

        // On ajoute le montant au solde
        compte.Solde += montant;

        // On indique au repository que le compte a été modifié
        compteRepository.Update(compte);

        // On enregistre aussi une opération pour garder une trace du versement
        compteRepository.AddOperation(
            new Operation
            {
                Type = "Versement",
                Montant = montant,
                DateOperation = DateTime.UtcNow,
                CompteId = compte.Id,
            }
        );

        // On sauvegarde les modifications en base
        compteRepository.SaveChanges();

        // On retourne le nouveau solde
        return compte.Solde;
    }

    // Effectue un retrait sur le compte
    public decimal EffectuerRetrait(string numeroCarte, string pin, decimal montant)
    {
        // Règle métier : un retrait doit être strictement positif
        if (montant <= 0)
        {
            throw new DomainValidationException("Le montant doit être positif");
        }

        // On authentifie la carte
        var carte = AuthentifierCarte(numeroCarte, pin);

        // On récupère le compte associé
        var compte = carte.Compte;

        // Règle métier : on interdit le découvert
        if (montant > compte.Solde)
        {
            throw new DomainValidationException("Solde insuffisant");
        }

        // On retire le montant du solde
        compte.Solde -= montant;

        // On indique que le compte a été modifié
        compteRepository.Update(compte);

        // On enregistre une opération de retrait
        compteRepository.AddOperation(
            new Operation
            {
                Type = "Retrait",
                Montant = montant,
                DateOperation = DateTime.UtcNow,
                CompteId = compte.Id,
            }
        );

        // On sauvegarde les modifications
        compteRepository.SaveChanges();

        // On retourne le nouveau solde
        return compte.Solde;
    }

    // Réinitialise l'état de l'automate pour les tests
    public void Reset()
    {
        // On récupère la carte de test demandée dans l'énoncé
        var carte = carteRepository.GetByNumeroCarte("123456");

        // Si elle n'existe pas, on lève une exception
        if (carte is null)
        {
            throw new NotFoundException("Carte de test introuvable");
        }

        // On récupère le compte lié à cette carte
        var compte = carte.Compte;

        // On remet le solde initial
        compte.Solde = 100.0m;

        // On remet le PIN initial
        carte.Pin = "0000";

        // On débloque la carte
        carte.EstBloquee = false;

        // On remet le nombre d'essais à 3
        carte.NombreEssaisRestants = 3;

        // On vide l'historique des opérations
        compte.Operations.Clear();

        // On signale que le compte et la carte ont été modifiés
        compteRepository.Update(compte);
        carteRepository.Update(carte);

        // On sauvegarde en base
        compteRepository.SaveChanges();
    }

    // Méthode privée utilisée par toutes les opérations sensibles
    // Elle vérifie la carte, le PIN, les essais restants et le blocage
    private CarteBancaire AuthentifierCarte(string numeroCarte, string pin)
    {
        // On cherche la carte en base via le repository
        var carte = carteRepository.GetByNumeroCarte(numeroCarte);

        // Si la carte n'existe pas, on renvoie une erreur 404 côté controller
        if (carte is null)
        {
            throw new NotFoundException("Carte introuvable");
        }

        // Si la carte est déjà bloquée, l'action est interdite
        if (carte.EstBloquee)
        {
            throw new ForbiddenException("Action impossible : carte bloquée");
        }

        // Si le PIN est incorrect
        if (carte.Pin != pin)
        {
            // On enlève un essai
            carte.NombreEssaisRestants--;

            // Si aucun essai ne reste, on bloque la carte
            if (carte.NombreEssaisRestants <= 0)
            {
                carte.NombreEssaisRestants = 0;
                carte.EstBloquee = true;

                // On sauvegarde le blocage de la carte
                carteRepository.Update(carte);
                carteRepository.SaveChanges();

                // Le controller transformera cette exception en 403 Forbidden
                throw new ForbiddenException("Carte bloquée");
            }

            // S'il reste encore des essais, on sauvegarde le compteur décrémenté
            carteRepository.Update(carte);
            carteRepository.SaveChanges();

            // Gestion du singulier/pluriel dans le message
            var motEssai = carte.NombreEssaisRestants == 1 ? "essai restant" : "essais restants";

            // Le controller transformera cette exception en 401 Unauthorized
            throw new UnauthorizedException(
                $"PIN incorrect. {carte.NombreEssaisRestants} {motEssai}"
            );
        }

        // Si le PIN est correct après une ou plusieurs erreurs,
        // on remet le compteur à 3
        if (carte.NombreEssaisRestants != 3)
        {
            carte.NombreEssaisRestants = 3;
            carteRepository.Update(carte);
            carteRepository.SaveChanges();
        }

        // Si tout est bon, on renvoie la carte authentifiée
        return carte;
    }
}