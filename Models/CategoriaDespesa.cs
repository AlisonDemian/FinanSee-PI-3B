namespace finansee_api.Models;

public class CategoriaDespesa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<NaturezaDespesa> Naturezas { get; set; } = new List<NaturezaDespesa>();
}
