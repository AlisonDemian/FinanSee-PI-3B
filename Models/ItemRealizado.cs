namespace finansee_api.Models;

public class ItemRealizado
{
    public int Id { get; set; }
    public int? ItemOrcadoId { get; set; }
    public int ProjetoId { get; set; }
    public int? NaturezaDespesaId { get; set; }
    public DateOnly DataDespesa { get; set; }
    public string? NumeroDocumento { get; set; }
    public decimal QuantidadeRealizada { get; set; }
    public decimal ValorUnitarioRealizado { get; set; }
    public decimal ValorTotalRealizado { get; set; }
    public string? Fornecedor { get; set; }
    public string? Justificativa { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public int UsuarioRegistroId { get; set; }

    public ItemOrcado? ItemOrcado { get; set; }
    public Projeto? Projeto { get; set; }
    public NaturezaDespesa? NaturezaDespesa { get; set; }
    public Usuario? UsuarioRegistro { get; set; }
}
