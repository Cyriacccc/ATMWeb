using ATMWeb.Data;
using ATMWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace ATMWeb.Repositories;

public class CompteRepository(DataContext context) : ICompteRepository
{
    public Compte? GetById(int id)
    {
        return context.Comptes
            .Include(c => c.CarteBancaire)
            .Include(c => c.Operations)
            .FirstOrDefault(c => c.Id == id);
    }

    public void Update(Compte compte)
    {
        context.Comptes.Update(compte);
    }

    public void AddOperation(Operation operation)
    {
        context.Operations.Add(operation);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}