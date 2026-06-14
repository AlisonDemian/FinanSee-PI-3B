namespace finansee_api.DTOs;

public class DashboardProjetoResponse
{
    public string Nome { get; set; } = string.Empty;
    public decimal Orcamento { get; set; }
    public decimal Gasto { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DashboardResponse
{
    public int ProjetosAtivos { get; set; }
    public decimal OrcamentoTotal { get; set; }
    public decimal GastoTotal { get; set; }
    public int Alertas { get; set; }
    public decimal SaldoDisponivel { get; set; }
    public decimal CusteioTotal { get; set; }
    public decimal CapitalTotal { get; set; }
    public IReadOnlyCollection<DashboardProjetoResponse> Projetos { get; set; } =
        Array.Empty<DashboardProjetoResponse>();
    public IReadOnlyCollection<string> GraficoLabels { get; set; } =
        Array.Empty<string>();
    public IReadOnlyCollection<decimal> GraficoOrcamento { get; set; } =
        Array.Empty<decimal>();
    public IReadOnlyCollection<decimal> GraficoGasto { get; set; } =
        Array.Empty<decimal>();
}

public class RelatorioResponse
{
    public int TotalProjetos { get; set; }
    public int EmAndamento { get; set; }
    public int Concluidos { get; set; }
    public decimal TotalGasto { get; set; }
}

public class EvolucaoResponse
{
    public IReadOnlyCollection<string> Labels { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<decimal> Orcado { get; set; } = Array.Empty<decimal>();
    public IReadOnlyCollection<decimal> Realizado { get; set; } = Array.Empty<decimal>();
}
