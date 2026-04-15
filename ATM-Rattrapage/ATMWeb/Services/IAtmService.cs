namespace ATMWeb.Services;

public interface IAtmService
{
    decimal ConsulterSolde(string numeroCarte, string pin);
    decimal EffectuerVersement(string numeroCarte, string pin, decimal montant);
    decimal EffectuerRetrait(string numeroCarte, string pin, decimal montant);
    void Reset();
}