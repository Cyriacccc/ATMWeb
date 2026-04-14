using ATMWeb.Model;

namespace ATMWeb.Repositories;

public interface ICompteRepository
{
    Compte? GetById(int id);

    void Update(Compte compte);

    void AddOperation(Operation operation);

    void SaveChanges();
}