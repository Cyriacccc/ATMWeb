namespace ATMWeb.Model;

public class Compte
{
    public int Id { get; set; }
    public decimal Solde { get; set; }

    public CarteBancaire CarteBancaire { get; set; } = null!;
    public List<Operation> Operations { get; set; } = [];
}
