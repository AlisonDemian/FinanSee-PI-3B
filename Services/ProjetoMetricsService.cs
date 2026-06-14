using finansee_api.Data;
using finansee_api.DTOs;
using finansee_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finansee_api.Services;

public class ProjetoMetricsService(ApplicationDbContext dbContext) : IProjetoMetricsService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public IReadOnlyCollection<ProjetoResumoResponse> GetProjetos(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.Projetos
            .Include(p => p.ItensOrcados)
            .Include(p => p.ItensRealizados)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(p => p.UsuarioResponsavelId == usuarioId);
        }

        return query
            .OrderBy(p => p.Titulo)
            .Select(p => MapResumo(p))
            .ToList();
    }

    public ProjetoDetalheResponse? GetProjetoDetalhe(int projetoId, int usuarioId, bool isAdmin, string? status)
    {
        var query = _dbContext.Projetos
            .Include(p => p.ItensOrcados)
                .ThenInclude(i => i.NaturezaDespesa)
                    .ThenInclude(n => n!.CategoriaDespesa)
            .Include(p => p.ItensRealizados)
                .ThenInclude(r => r.ItemOrcado)
                    .ThenInclude(i => i!.NaturezaDespesa)
                        .ThenInclude(n => n!.CategoriaDespesa)
            .Include(p => p.ItensRealizados)
                .ThenInclude(r => r.NaturezaDespesa)
                    .ThenInclude(n => n!.CategoriaDespesa)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(p => p.UsuarioResponsavelId == usuarioId);
        }

        var projeto = query.FirstOrDefault(p => p.Id == projetoId);

        if (projeto is null)
        {
            return null;
        }

        var itensOrcados = projeto.ItensOrcados.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            itensOrcados = status == "concluido"
                ? itensOrcados.Where(i => i.Status == ItemOrcadoStatus.Concluido)
                : itensOrcados.Where(i => i.Status == ItemOrcadoStatus.NaoIniciado);
        }

        var totalOrcado = projeto.ItensOrcados.Sum(i => i.ValorTotalOrcado);
        var totalRealizado = projeto.ItensRealizados.Sum(r => r.ValorTotalRealizado);
        var disponivel = totalOrcado - totalRealizado;
        var percentual = totalOrcado > 0 ? Math.Round(totalRealizado / totalOrcado * 100, 2) : 0;

        var custeio = projeto.ItensRealizados
            .Where(r => (r.ItemOrcado?.NaturezaDespesa?.CategoriaDespesa?.Nome ?? r.NaturezaDespesa?.CategoriaDespesa?.Nome) == "Custeio")
            .Sum(r => r.ValorTotalRealizado);

        var capital = projeto.ItensRealizados
            .Where(r => (r.ItemOrcado?.NaturezaDespesa?.CategoriaDespesa?.Nome ?? r.NaturezaDespesa?.CategoriaDespesa?.Nome) == "Capital")
            .Sum(r => r.ValorTotalRealizado);

        var naturezas = projeto.ItensOrcados
            .GroupBy(i => i.NaturezaDespesa?.Nome ?? string.Empty)
            .Select(g => new
            {
                Label = g.Key,
                Orcado = g.Sum(i => i.ValorTotalOrcado),
                Realizado = projeto.ItensRealizados
                    .Where(r => r.ItemOrcado?.NaturezaDespesaId != null
                        ? r.ItemOrcado.NaturezaDespesa?.Nome == g.Key
                        : r.NaturezaDespesa?.Nome == g.Key)
                    .Sum(r => r.ValorTotalRealizado)
            })
            .ToList();

        return new ProjetoDetalheResponse
        {
            Id = projeto.Id,
            Titulo = projeto.Titulo,
            NumeroEdital = projeto.NumeroEdital,
            DataInicio = projeto.DataInicio,
            DataFim = projeto.DataFim,
            ValorTotalOrcamento = projeto.ValorTotalOrcamento,
            TotalOrcado = totalOrcado,
            Planejado = projeto.ValorTotalOrcamento,
            TotalRealizado = totalRealizado,
            Disponivel = disponivel,
            Percentual = percentual,
            Custeio = custeio,
            Capital = capital,
            Labels = naturezas.Select(n => n.Label).ToList(),
            Orcado = naturezas.Select(n => n.Orcado).ToList(),
            Realizado = naturezas.Select(n => n.Realizado).ToList(),
            ItensOrcados = itensOrcados.Select(MapItemOrcado).ToList(),
            ItensRealizados = projeto.ItensRealizados.Select(MapGasto).ToList()
        };
    }

    public DashboardResponse GetDashboard(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.Projetos
            .Include(p => p.ItensOrcados)
            .Include(p => p.ItensRealizados)
                .ThenInclude(r => r.ItemOrcado)
                    .ThenInclude(i => i!.NaturezaDespesa)
                        .ThenInclude(n => n!.CategoriaDespesa)
            .Include(p => p.ItensRealizados)
                .ThenInclude(r => r.NaturezaDespesa)
                    .ThenInclude(n => n!.CategoriaDespesa)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(p => p.UsuarioResponsavelId == usuarioId);
        }

        var projetos = query.OrderBy(p => p.Titulo).ToList();

        var orcamentoTotal = projetos.Sum(p => p.ValorTotalOrcamento);
        var gastoTotal = projetos.Sum(p => p.ItensRealizados.Sum(r => r.ValorTotalRealizado));

        var custeioTotal = projetos.SelectMany(p => p.ItensRealizados)
            .Where(r => (r.ItemOrcado?.NaturezaDespesa?.CategoriaDespesa?.Nome ?? r.NaturezaDespesa?.CategoriaDespesa?.Nome) == "Custeio")
            .Sum(r => r.ValorTotalRealizado);

        var capitalTotal = projetos.SelectMany(p => p.ItensRealizados)
            .Where(r => (r.ItemOrcado?.NaturezaDespesa?.CategoriaDespesa?.Nome ?? r.NaturezaDespesa?.CategoriaDespesa?.Nome) == "Capital")
            .Sum(r => r.ValorTotalRealizado);

        var alertas = projetos.Count(p =>
        {
            var orcado = p.ItensOrcados.Sum(i => i.ValorTotalOrcado);
            var realizado = p.ItensRealizados.Sum(r => r.ValorTotalRealizado);
            return orcado > 0 && realizado > orcado;
        });

        var projetosResponse = projetos.Select(p =>
        {
            var orcado = p.ItensOrcados.Sum(i => i.ValorTotalOrcado);
            var gasto = p.ItensRealizados.Sum(r => r.ValorTotalRealizado);
            return new DashboardProjetoResponse
            {
                Nome = p.Titulo,
                Orcamento = orcado,
                Gasto = gasto,
                Status = ResolverStatus(p)
            };
        }).ToList();

        return new DashboardResponse
        {
            ProjetosAtivos = projetos.Count,
            OrcamentoTotal = orcamentoTotal,
            GastoTotal = gastoTotal,
            Alertas = alertas,
            SaldoDisponivel = orcamentoTotal - gastoTotal,
            CusteioTotal = custeioTotal,
            CapitalTotal = capitalTotal,
            Projetos = projetosResponse,
            GraficoLabels = projetos.Select(p => p.Titulo).ToList(),
            GraficoOrcamento = projetos.Select(p => p.ItensOrcados.Sum(i => i.ValorTotalOrcado)).ToList(),
            GraficoGasto = projetos.Select(p => p.ItensRealizados.Sum(r => r.ValorTotalRealizado)).ToList()
        };
    }

    public RelatorioResponse GetRelatorio(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.Projetos
            .Include(p => p.ItensRealizados)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(p => p.UsuarioResponsavelId == usuarioId);
        }

        var projetos = query.ToList();
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        return new RelatorioResponse
        {
            TotalProjetos = projetos.Count,
            EmAndamento = projetos.Count(p => p.DataFim == null || p.DataFim >= hoje),
            Concluidos = projetos.Count(p => p.DataFim.HasValue && p.DataFim < hoje),
            TotalGasto = projetos.Sum(p => p.ItensRealizados.Sum(r => r.ValorTotalRealizado))
        };
    }

    public EvolucaoResponse? GetEvolucao(int projetoId, int usuarioId, bool isAdmin, string tipo)
    {
        var query = _dbContext.Projetos
            .Include(p => p.ItensOrcados)
            .Include(p => p.ItensRealizados)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(p => p.UsuarioResponsavelId == usuarioId);
        }

        var projeto = query.FirstOrDefault(p => p.Id == projetoId);

        if (projeto is null)
        {
            return null;
        }

        var gastos = projeto.ItensRealizados.OrderBy(r => r.DataDespesa).ToList();

        if (gastos.Count == 0)
        {
            return new EvolucaoResponse();
        }

        var totalOrcado = projeto.ItensOrcados.Sum(i => i.ValorTotalOrcado);

        IEnumerable<IGrouping<string, ItemRealizado>> grupos = tipo == "semanal"
            ? gastos.GroupBy(r => $"Sem {GetSemana(r.DataDespesa)}/{r.DataDespesa.Year}")
            : gastos.GroupBy(r => $"{NomeMes(r.DataDespesa.Month)}/{r.DataDespesa.Year}");

        var agrupados = grupos.ToList();

        return new EvolucaoResponse
        {
            Labels = agrupados.Select(g => g.Key).ToList(),
            Realizado = agrupados.Select(g => g.Sum(r => r.ValorTotalRealizado)).ToList(),
            Orcado = agrupados.Select(_ => totalOrcado).ToList()
        };
    }

    private static ProjetoResumoResponse MapResumo(Projeto p)
    {
        var orcado = p.ItensOrcados.Sum(i => i.ValorTotalOrcado);
        var realizado = p.ItensRealizados.Sum(r => r.ValorTotalRealizado);
        var percentual = orcado > 0 ? Math.Round(realizado / orcado * 100, 2) : 0;

        return new ProjetoResumoResponse
        {
            Id = p.Id,
            Titulo = p.Titulo,
            NumeroEdital = p.NumeroEdital,
            DataInicio = p.DataInicio,
            DataFim = p.DataFim,
            ValorTotalOrcamento = p.ValorTotalOrcamento,
            Orcado = orcado,
            Realizado = realizado,
            Percentual = percentual,
            Status = ResolverStatus(p),
            CriadoEm = p.CriadoEm
        };
    }

    private static ItemOrcadoResponse MapItemOrcado(ItemOrcado item)
    {
        return new ItemOrcadoResponse
        {
            Id = item.Id,
            Status = item.Status == ItemOrcadoStatus.Concluido ? "concluido" : "nao_iniciado",
            ProjetoId = item.ProjetoId,
            NaturezaDespesaId = item.NaturezaDespesaId,
            Natureza = item.NaturezaDespesa?.Nome ?? string.Empty,
            Categoria = item.NaturezaDespesa?.CategoriaDespesa?.Nome ?? string.Empty,
            Descricao = item.Descricao,
            QuantidadeOrcada = item.QuantidadeOrcada,
            ValorUnitarioOrcado = item.ValorUnitarioOrcado,
            ValorTotalOrcado = item.ValorTotalOrcado,
            CriadoEm = item.CriadoEm
        };
    }

    private static GastoResponse MapGasto(ItemRealizado gasto)
    {
        return new GastoResponse
        {
            Id = gasto.Id,
            ProjetoId = gasto.ProjetoId,
            ItemOrcadoId = gasto.ItemOrcadoId,
            NaturezaDespesaId = gasto.NaturezaDespesaId,
            Natureza = gasto.ItemOrcado?.NaturezaDespesa?.Nome ?? gasto.NaturezaDespesa?.Nome,
            TipoDespesa = gasto.ItemOrcadoId.HasValue ? "Previsto" : "Imprevisto",
            DataDespesa = gasto.DataDespesa,
            QuantidadeRealizada = gasto.QuantidadeRealizada,
            ValorUnitarioRealizado = gasto.ValorUnitarioRealizado,
            ValorTotalRealizado = gasto.ValorTotalRealizado,
            NumeroDocumento = gasto.NumeroDocumento,
            Fornecedor = gasto.Fornecedor,
            Justificativa = gasto.Justificativa,
            Observacao = gasto.Observacao,
            CriadoEm = gasto.CriadoEm
        };
    }

    private static string ResolverStatus(Projeto p)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        return p.DataFim.HasValue && p.DataFim < hoje ? "Concluído" : "Em andamento";
    }

    private static int GetSemana(DateOnly data)
    {
        return System.Globalization.ISOWeek.GetWeekOfYear(data.ToDateTime(TimeOnly.MinValue));
    }

    private static string NomeMes(int mes) => mes switch
    {
        1 => "Jan", 2 => "Fev", 3 => "Mar", 4 => "Abr",
        5 => "Mai", 6 => "Jun", 7 => "Jul", 8 => "Ago",
        9 => "Set", 10 => "Out", 11 => "Nov", 12 => "Dez",
        _ => mes.ToString()
    };
}
