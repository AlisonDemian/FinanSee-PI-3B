using finansee_api.DTOs;
using finansee_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finansee_api.Controllers;

[ApiController]
[Authorize]
[Route("api/relatorios")]
public class RelatoriosController(IProjetoMetricsService projetoMetricsService) : ApiControllerBase
{
    private readonly IProjetoMetricsService _projetoMetricsService = projetoMetricsService;

    [HttpGet]
    public ActionResult<RelatorioResponse> Get()
    {
        var response = _projetoMetricsService.GetRelatorio(CurrentUserId, IsAdmin);

        return Ok(response);
    }

    [HttpGet("evolucao")]
    public ActionResult<EvolucaoResponse> GetEvolucao(
        [FromQuery] int projetoId,
        [FromQuery] string tipo = "mensal")
    {
        var response = _projetoMetricsService.GetEvolucao(projetoId, CurrentUserId, IsAdmin, tipo);

        return response is null ? NotFound() : Ok(response);
    }
}
