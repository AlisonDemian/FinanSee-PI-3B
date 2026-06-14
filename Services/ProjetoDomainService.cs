using finansee_api.Data;
using finansee_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace finansee_api.Services;

public partial class ProjetoDomainService(ApplicationDbContext dbContext) : IProjetoDomainService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Projeto? ObterProjeto(
        int projetoId,
        int usuarioId,
        bool isAdmin)
    {
        return FiltrarProjetos(usuarioId, isAdmin)
            .FirstOrDefault(x => x.Id == projetoId);
    }

    public ItemOrcado? ObterItemOrcado(
        int itemId,
        int usuarioId,
        bool isAdmin)
    {
        return FiltrarItensOrcados(usuarioId, isAdmin)
            .Include(x => x.NaturezaDespesa)
            .ThenInclude(x => x!.CategoriaDespesa)
            .Include(x => x.Projeto)
            .FirstOrDefault(x => x.Id == itemId);
    }

    public ItemRealizado? ObterGasto(
        int gastoId,
        int usuarioId,
        bool isAdmin)
    {
        return FiltrarGastos(usuarioId, isAdmin)
            .Include(x => x.ItemOrcado)
            .ThenInclude(x => x!.NaturezaDespesa)
            .ThenInclude(x => x!.CategoriaDespesa)
            .Include(x => x.NaturezaDespesa)
            .ThenInclude(x => x!.CategoriaDespesa)
            .Include(x => x.Projeto)
            .FirstOrDefault(x => x.Id == gastoId);
    }

    public void AtualizarStatusItem(int itemId)
    {
        var item = _dbContext.ItensOrcados
            .Include(x => x.ItensRealizados)
            .FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return;
        }

        item.Status = item.ItensRealizados.Count > 0
            ? ItemOrcadoStatus.Concluido
            : ItemOrcadoStatus.NaoIniciado;

        _dbContext.SaveChanges();
    }

    public string? NormalizarNumeroEdital(string? numeroEdital)
    {
        if (string.IsNullOrWhiteSpace(numeroEdital))
        {
            return null;
        }

        var valor = numeroEdital.Trim();
        var partes = NumeroRegex().Matches(valor).Select(x => x.Value).ToArray();

        if (partes.Length >= 2
            && int.TryParse(partes[0], out var numero)
            && int.TryParse(partes[1], out var ano))
        {
            var sufixo = TextoRegex().Replace(valor, string.Empty).Trim();
            var baseEdital = $"{numero:000}/{ano}";

            return string.IsNullOrWhiteSpace(sufixo)
                ? baseEdital
                : $"{sufixo.ToUpperInvariant()} {baseEdital}";
        }

        return valor.ToUpperInvariant();
    }

    private IQueryable<Projeto> FiltrarProjetos(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.Projetos.AsQueryable();
        return isAdmin ? query : query.Where(x => x.UsuarioResponsavelId == usuarioId);
    }

    private IQueryable<ItemOrcado> FiltrarItensOrcados(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.ItensOrcados.AsQueryable();

        return isAdmin
            ? query
            : query.Where(x => x.Projeto!.UsuarioResponsavelId == usuarioId);
    }

    private IQueryable<ItemRealizado> FiltrarGastos(int usuarioId, bool isAdmin)
    {
        var query = _dbContext.ItensRealizados.AsQueryable();

        return isAdmin
            ? query
            : query.Where(x => x.Projeto!.UsuarioResponsavelId == usuarioId);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumeroRegex();

    [GeneratedRegex(@"[\d+/]+")]
    private static partial Regex TextoRegex();
}
