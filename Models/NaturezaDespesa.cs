namespace finansee_api.Models;

public class NaturezaDespesa
{
    public int Id { get; set; }
    public int CategoriaDespesaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? UnidadeMedida { get; set; }

    public CategoriaDespesa? CategoriaDespesa { get; set; }
    public ICollection<ItemOrcado> ItensOrcados { get; set; } = new List<ItemOrcado>();
    public ICollection<ItemRealizado> ItensRealizados { get; set; } = new List<ItemRealizado>();
}
