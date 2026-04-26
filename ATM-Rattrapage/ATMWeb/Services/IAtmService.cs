namespace ATMWeb.Services;

// Interface du service ATM
// Elle définit les opérations métier disponibles pour l’automate bancaire
public interface IAtmService
{
    // Permet de consulter le solde d’un compte
    // Nécessite le numéro de carte et le PIN pour authentification
    decimal ConsulterSolde(string numeroCarte, string pin);

    // Permet d’effectuer un versement sur le compte
    // Le montant doit être positif
    decimal EffectuerVersement(string numeroCarte, string pin, decimal montant);

    // Permet d’effectuer un retrait sur le compte
    // Le montant doit être positif et inférieur au solde
    decimal EffectuerRetrait(string numeroCarte, string pin, decimal montant);

    // Permet de réinitialiser l’état de l’ATM
    // (utilisé pour les tests : carte, PIN, solde, essais)
    void Reset();
}