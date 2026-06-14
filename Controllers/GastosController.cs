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
public class GastosController(
    ApplicationDbContext dbContext,
    IProjetoDomainService projetoDomainService) : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IProjetoDomainService _projetoDomainService = projetoDomainService;

    [HttpGet("gastos/{id:int}")]
    public ActionResult<GastoResponse> GetById(int id)
    {
        var gasto = _projetoDomainService.ObterGasto(id, CurrentUserId, IsAdmin);

        return gasto is null ? NotFound() : Ok(MapGasto(gasto));
    }

    [HttpPost("projetos/{projetoId:int}/gastos")]
    public ActionResult<GastoResponse> Create(
        int projetoId,
        [FromBody] GastoUpsertRequest request)
    {
        var projeto = _projetoDomainService.ObterProjeto(projetoId, CurrentUserId, IsAdmin);

        if (projeto is null)
        {
            return NotFound();
        }

        var (itemOrcado, naturezaDespesa, errors) = ResolveDependencies(projetoId, request);

        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var gasto = new ItemRealizado
        {
            ProjetoId = projetoId,
            ItemOrcadoId = itemOrcado?.Id,
            NaturezaDespesaId = itemOrcado?.NaturezaDespesaId ?? naturezaDespesa?.Id,
            DataDespesa = request.DataDespesa,
            QuantidadeRealizada = request.QuantidadeRealizada,
            ValorUnitarioRealizado = request.ValorUnitarioRealizado,
            ValorTotalRealizado = request.QuantidadeRealizada * request.ValorUnitarioRealizado,
            NumeroDocumento = request.NumeroDocumento?.Trim(),
            Fornecedor = request.Fornecedor?.Trim(),
            Justificativa = request.Justificativa?.Trim(),
            Observacao = request.Observacao?.Trim(),
            UsuarioRegistroId = CurrentUserId
        };

        _dbContext.ItensRealizados.Add(gasto);
        _dbContext.SaveChanges();

        if (itemOrcado is not null)
        {
            _projetoDomainService.AtualizarStatusItem(itemOrcado.Id);
        }

        var response = _projetoDomainService.ObterGasto(gasto.Id, CurrentUserId, IsAdmin);

        return Ok(MapGasto(response!));
    }

    [HttpPut("gastos/{id:int}")]
    public ActionResult<GastoResponse> Update(
        int id,
        [FromBody] GastoUpsertRequest request)
    {
        var gasto = _projetoDomainService.ObterGasto(id, CurrentUserId, IsAdmin);

        if (gasto is null)
        {
            return NotFound();
        }

        var oldItemOrcadoId = gasto.ItemOrcadoId;
        var (itemOrcado, naturezaDespesa, errors) = ResolveDependencies(gasto.ProjetoId, request);

        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        gasto.ItemOrcadoId = itemOrcado?.Id;
        gasto.NaturezaDespesaId = itemOrcado?.NaturezaDespesaId ?? naturezaDespesa?.Id;
        gasto.DataDespesa = request.DataDespesa;
        gasto.QuantidadeRealizada = request.QuantidadeRealizada;
        gasto.ValorUnitarioRealizado = request.ValorUnitarioRealizado;
        gasto.ValorTotalRealizado = request.QuantidadeRealizada * request.ValorUnitarioRealizado;
        gasto.NumeroDocumento = request.NumeroDocumento?.Trim();
        gasto.Fornecedor = request.Fornecedor?.Trim();
        gasto.Justificativa = request.Justificativa?.Trim();
        gasto.Observacao = request.Observacao?.Trim();

        _dbContext.SaveChanges();

        if (oldItemOrcadoId.HasValue)
        {
            _projetoDomainService.AtualizarStatusItem(oldItemOrcadoId.Value);
        }

        if (itemOrcado is not null && itemOrcado.Id != oldItemOrcadoId)
        {
            _projetoDomainService.AtualizarStatusItem(itemOrcado.Id);
        }

        var response = _projetoDomainService.ObterGasto(gasto.Id, CurrentUserId, IsAdmin);

        return Ok(MapGasto(response!));
    }

    [HttpDelete("gastos/{id:int}")]
    public IActionResult Delete(int id)
    {
        var gasto = _projetoDomainService.ObterGasto(id, CurrentUserId, IsAdmin);

        if (gasto is null)
        {
            return NotFound();
        }

        var itemOrcadoId = gasto.ItemOrcadoId;

        _dbContext.ItensRealizados.Remove(gasto);
        _dbContext.SaveChanges();

        if (itemOrcadoId.HasValue)
        {
            _projetoDomainService.AtualizarStatusItem(itemOrcadoId.Value);
        }

        return NoContent();
    }

    private (ItemOrcado? ItemOrcado, NaturezaDespesa? NaturezaDespesa, Dictionary<string, string> Errors) ResolveDependencies(
        int projetoId,
        GastoUpsertRequest request)
    {
        var errors = new Dictionary<string, string>();

        if (request.DataDespesa == default)
        {
            errors[nameof(request.DataDespesa)] = "A data da despesa é obrigatória.";
        }

        if (request.QuantidadeRealizada <= 0)
        {
            errors[nameof(request.QuantidadeRealizada)] = "A quantidade realizada deve ser positiva.";
        }

        if (request.ValorUnitarioRealizado <= 0)
        {
            errors[nameof(request.ValorUnitarioRealizado)] = "O valor unitário realizado deve ser positivo.";
        }

        if (request.ItemOrcadoId.HasValue)
        {
            var item = _dbContext.ItensOrcados
                .Include(x => x.NaturezaDespesa)
                .ThenInclude(x => x!.CategoriaDespesa)
                .FirstOrDefault(
                    x => x.Id == request.ItemOrcadoId.Value && x.ProjetoId == projetoId);

            if (item is null)
            {
                errors[nameof(request.ItemOrcadoId)] = "O item orçado informado não pertence ao projeto.";
            }

            return (item, null, errors);
        }

        if (!request.NaturezaDespesaId.HasValue)
        {
            errors[nameof(request.NaturezaDespesaId)] = "Informe a natureza para gastos imprevistos.";
        }

        if (string.IsNullOrWhiteSpace(request.Justificativa))
        {
            errors[nameof(request.Justificativa)] = "Informe a justificativa para gastos imprevistos.";
        }

        NaturezaDespesa? natureza = null;

        if (request.NaturezaDespesaId.HasValue)
        {
            natureza = _dbContext.NaturezasDespesa
                .Include(x => x.CategoriaDespesa)
                .FirstOrDefault(x => x.Id == request.NaturezaDespesaId.Value);

            if (natureza is null)
            {
                errors[nameof(request.NaturezaDespesaId)] = "Natureza da despesa não encontrada.";
            }
        }

        return (null, natureza, errors);
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
}
