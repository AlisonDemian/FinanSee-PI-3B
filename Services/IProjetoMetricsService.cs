using finansee_api.DTOs;

namespace finansee_api.Services;

public interface IProjetoMetricsService
{
    IReadOnlyCollection<ProjetoResumoResponse> GetProjetos(int usuarioId, bool isAdmin);
    ProjetoDetalheResponse? GetProjetoDetalhe(int projetoId, int usuarioId, bool isAdmin, string? status);
    DashboardResponse GetDashboard(int usuarioId, bool isAdmin);
    RelatorioResponse GetRelatorio(int usuarioId, bool isAdmin);
    EvolucaoResponse? GetEvolucao(int projetoId, int usuarioId, bool isAdmin, string tipo);
}
