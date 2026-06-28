using System.ComponentModel.DataAnnotations;

namespace finansee_api.DTOs;

public class ProjetoUpsertRequest
{
    [Required, MinLength(3), MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NumeroEdital { get; set; }

    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }

    public decimal ValorTotalOrcamento { get; set; }

    public int? UsuarioResponsavelId { get; set; }
}

public class ProjetoResumoResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? NumeroEdital { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public decimal ValorTotalOrcamento { get; set; }
    public decimal Orcado { get; set; }
    public decimal Realizado { get; set; }
    public decimal Percentual { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}

public class ProjetoDetalheResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? NumeroEdital { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public decimal ValorTotalOrcamento { get; set; }
    public decimal TotalOrcado { get; set; }
    public decimal Planejado { get; set; }
    public decimal TotalRealizado { get; set; }
    public decimal Disponivel { get; set; }
    public decimal Percentual { get; set; }
    public decimal Custeio { get; set; }
    public decimal Capital { get; set; }
    public IReadOnlyCollection<string> Labels { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<decimal> Orcado { get; set; } = Array.Empty<decimal>();
    public IReadOnlyCollection<decimal> Realizado { get; set; } = Array.Empty<decimal>();
    public IReadOnlyCollection<ItemOrcadoResponse> ItensOrcados { get; set; } =
        Array.Empty<ItemOrcadoResponse>();
    public IReadOnlyCollection<GastoResponse> ItensRealizados { get; set; } =
        Array.Empty<GastoResponse>();
}
