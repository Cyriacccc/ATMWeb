namespace ATMWeb.Model;

public class CarteBancaire
{
    public int Id { get; set; }
    public string NumeroCarte { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public bool EstBloquee { get; set; }
    public int NombreEssaisRestants { get; set; } = 3;

    public int CompteId { get; set; }
    public Compte Compte { get; set; } = null!;
}