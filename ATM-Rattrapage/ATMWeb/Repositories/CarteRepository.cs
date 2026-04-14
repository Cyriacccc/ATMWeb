using ATMWeb.Data;
using ATMWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Repositories;

public class CarteRepository(DataContext context) : ICarteRepository
{
    public CarteBancaire? GetByNumeroCarte(string numeroCarte)
    {
        return context.CartesBancaires
            .Include(c => c.Compte)
            .ThenInclude(c => c.Operations)
            .FirstOrDefault(c => c.NumeroCarte == numeroCarte);
    }

    public void Update(CarteBancaire carte)
    {
        context.CartesBancaires.Update(carte);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}