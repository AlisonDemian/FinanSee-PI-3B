using finansee_api.Data;
using finansee_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finansee_api.Controllers;

[ApiController]
[Authorize]
[Route("api/catalogos")]
public class CatalogosController(ApplicationDbContext dbContext) : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet("categorias")]
    public ActionResult<IReadOnlyCollection<CategoriaResponse>> GetCategorias()
    {
        var categorias = _dbContext.CategoriasDespesa
            .AsNoTracking()
            .OrderBy(x => x.Nome)
            .Select(x => new CategoriaResponse
            {
                Id = x.Id,
                Nome = x.Nome,
                Descricao = x.Descricao
            })
            .ToList();

        return Ok(categorias);
    }

    [HttpGet("naturezas")]
    public ActionResult<IReadOnlyCollection<NaturezaResponse>> GetNaturezas(
        [FromQuery] int? categoriaId)
    {
        var query = _dbContext.NaturezasDespesa
            .AsNoTracking()
            .Include(x => x.CategoriaDespesa)
            .AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaDespesaId == categoriaId.Value);
        }

        var naturezas = query
            .OrderBy(x => x.Nome)
            .Select(x => new NaturezaResponse
            {
                Id = x.Id,
                CategoriaId = x.CategoriaDespesaId,
                Categoria = x.CategoriaDespesa!.Nome,
                Nome = x.Nome,
                UnidadeMedida = x.UnidadeMedida
            })
            .ToList();

        return Ok(naturezas);
    }
}
