using finansee_api.Models;

namespace finansee_api.Services;

public interface IProjetoDomainService
{
    Projeto? ObterProjeto(int projetoId, int usuarioId, bool isAdmin);
    ItemOrcado? ObterItemOrcado(int itemId, int usuarioId, bool isAdmin);
    ItemRealizado? ObterGasto(int gastoId, int usuarioId, bool isAdmin);
    void AtualizarStatusItem(int itemId);
    string? NormalizarNumeroEdital(string? numeroEdital);
}
