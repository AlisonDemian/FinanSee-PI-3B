using System.ComponentModel.DataAnnotations;

namespace finansee_api.DTOs;

public class ItemOrcadoUpsertRequest
{
    [Required]
    public int NaturezaDespesaId { get; set; }

    public string? Descricao { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999")]
    public decimal QuantidadeOrcada { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999")]
    public decimal ValorUnitarioOrcado { get; set; }
}

public class ItemOrcadoResponse
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProjetoId { get; set; }
    public int NaturezaDespesaId { get; set; }
    public string Natureza { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal QuantidadeOrcada { get; set; }
    public decimal ValorUnitarioOrcado { get; set; }
    public decimal ValorTotalOrcado { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class GastoUpsertRequest
{
    public int? ItemOrcadoId { get; set; }
    public int? NaturezaDespesaId { get; set; }
    public DateOnly DataDespesa { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999")]
    public decimal QuantidadeRealizada { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999")]
    public decimal ValorUnitarioRealizado { get; set; }

    public string? NumeroDocumento { get; set; }
    public string? Fornecedor { get; set; }
    public string? Justificativa { get; set; }
    public string? Observacao { get; set; }
}

public class GastoResponse
{
    public int Id { get; set; }
    public int ProjetoId { get; set; }
    public int? ItemOrcadoId { get; set; }
    public int? NaturezaDespesaId { get; set; }
    public string? Natureza { get; set; }
    public string TipoDespesa { get; set; } = string.Empty;
    public DateOnly DataDespesa { get; set; }
    public decimal QuantidadeRealizada { get; set; }
    public decimal ValorUnitarioRealizado { get; set; }
    public decimal ValorTotalRealizado { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Fornecedor { get; set; }
    public string? Justificativa { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
}
