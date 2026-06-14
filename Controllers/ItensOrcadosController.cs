using finansee_api.Data;
using finansee_api.DTOs;
using finansee_api.Models;
using finansee_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finansee_api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class ItensOrcadosController(
    ApplicationDbContext dbContext,
    IProjetoDomainService projetoDomainService) : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IProjetoDomainService _projetoDomainService = projetoDomainService;

    [HttpPost("projetos/{projetoId:int}/itens-orcados")]
    public ActionResult<ItemOrcadoResponse> Create(
        int projetoId,
        [FromBody] ItemOrcadoUpsertRequest request)
    {
        var projeto = _projetoDomainService.ObterProjeto(projetoId, CurrentUserId, IsAdmin);

        if (projeto is null)
        {
            return NotFound();
        }

        var natureza = _dbContext.NaturezasDespesa
            .AsNoTracking()
            .Include(x => x.CategoriaDespesa)
            .FirstOrDefault(x => x.Id == request.NaturezaDespesaId);

        if (natureza is null)
        {
            ModelState.AddModelError(nameof(request.NaturezaDespesaId), "Natureza da despesa não encontrada.");
            return ValidationProblem(ModelState);
        }

        var item = new ItemOrcado
        {
            ProjetoId = projeto.Id,
            NaturezaDespesaId = request.NaturezaDespesaId,
            Descricao = request.Descricao?.Trim(),
            QuantidadeOrcada = request.QuantidadeOrcada,
            ValorUnitarioOrcado = request.ValorUnitarioOrcado,
            ValorTotalOrcado = request.QuantidadeOrcada * request.ValorUnitarioOrcado
        };

        _dbContext.ItensOrcados.Add(item);
        _dbContext.SaveChanges();

        var response = _projetoDomainService.ObterItemOrcado(item.Id, CurrentUserId, IsAdmin);

        return Ok(MapItem(response!));
    }

    [HttpPut("itens-orcados/{id:int}")]
    public ActionResult<ItemOrcadoResponse> Update(
        int id,
        [FromBody] ItemOrcadoUpsertRequest request)
    {
        var item = _projetoDomainService.ObterItemOrcado(id, CurrentUserId, IsAdmin);

        if (item is null)
        {
            return NotFound();
        }

        var natureza = _dbContext.NaturezasDespesa
            .AsNoTracking()
            .Include(x => x.CategoriaDespesa)
            .FirstOrDefault(x => x.Id == request.NaturezaDespesaId);

        if (natureza is null)
        {
            ModelState.AddModelError(nameof(request.NaturezaDespesaId), "Natureza da despesa não encontrada.");
            return ValidationProblem(ModelState);
        }

        item.NaturezaDespesaId = request.NaturezaDespesaId;
        item.Descricao = request.Descricao?.Trim();
        item.QuantidadeOrcada = request.QuantidadeOrcada;
        item.ValorUnitarioOrcado = request.ValorUnitarioOrcado;
        item.ValorTotalOrcado = request.QuantidadeOrcada * request.ValorUnitarioOrcado;

        _dbContext.SaveChanges();

        var response = _projetoDomainService.ObterItemOrcado(item.Id, CurrentUserId, IsAdmin);

        return Ok(MapItem(response!));
    }

    [HttpDelete("itens-orcados/{id:int}")]
    public IActionResult Delete(int id)
    {
        var item = _projetoDomainService.ObterItemOrcado(id, CurrentUserId, IsAdmin);

        if (item is null)
        {
            return NotFound();
        }

        _dbContext.ItensOrcados.Remove(item);
        _dbContext.SaveChanges();

        return NoContent();
    }

    private static ItemOrcadoResponse MapItem(ItemOrcado item)
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
}
