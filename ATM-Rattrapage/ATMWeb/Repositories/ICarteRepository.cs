using ATMWeb.Model;

namespace ATMWeb.Repositories;

public interface ICarteRepository
{
    CarteBancaire? GetByNumeroCarte(string numeroCarte);
    void Update(CarteBancaire carte);
    void SaveChanges();
}