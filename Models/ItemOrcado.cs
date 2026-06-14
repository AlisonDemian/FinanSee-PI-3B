namespace finansee_api.Models;

public class ItemOrcado
{
    public int Id { get; set; }
    public ItemOrcadoStatus Status { get; set; } = ItemOrcadoStatus.NaoIniciado;
    public int ProjetoId { get; set; }
    public int NaturezaDespesaId { get; set; }
    public string? Descricao { get; set; }
    public decimal QuantidadeOrcada { get; set; }
    public decimal ValorUnitarioOrcado { get; set; }
    public decimal ValorTotalOrcado { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Projeto? Projeto { get; set; }
    public NaturezaDespesa? NaturezaDespesa { get; set; }
    public ICollection<ItemRealizado> ItensRealizados { get; set; } = new List<ItemRealizado>();
}
