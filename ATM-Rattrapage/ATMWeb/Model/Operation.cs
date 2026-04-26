namespace ATMWeb.Model;

public class Operation
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DateOperation { get; set; }

    public int CompteId { get; set; }
    public Compte Compte { get; set; } = null!;
}
