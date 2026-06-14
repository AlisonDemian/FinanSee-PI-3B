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
[Route("api/projetos")]
public class ProjetosController(
    ApplicationDbContext dbContext,
    IProjetoDomainService projetoDomainService,
    IProjetoMetricsService projetoMetricsService) : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IProjetoDomainService _projetoDomainService = projetoDomainService;
    private readonly IProjetoMetricsService _projetoMetricsService = projetoMetricsService;

    [HttpGet]
    public ActionResult<IReadOnlyCollection<ProjetoResumoResponse>> Get()
    {
        var projetos = _projetoMetricsService.GetProjetos(CurrentUserId, IsAdmin);

        return Ok(projetos);
    }

    [HttpGet("{id:int}")]
    public ActionResult<ProjetoDetalheResponse> GetById(
        int id,
        [FromQuery] string? status)
    {
        var projeto = _projetoMetricsService.GetProjetoDetalhe(id, CurrentUserId, IsAdmin, status);

        return projeto is null ? NotFound() : Ok(projeto);
    }

    [HttpPost]
    public ActionResult<ProjetoDetalheResponse> Create(
        [FromBody] ProjetoUpsertRequest request)
    {
        if (!ValidateProjetoRequest(request))
        {
            return ValidationProblem(ModelState);
        }

        var usuarioResponsavelId = CurrentUserId;

        if (IsAdmin && request.UsuarioResponsavelId.HasValue)
        {
            var usuarioExiste = _dbContext.Usuarios
                .Any(x => x.Id == request.UsuarioResponsavelId.Value);

            if (!usuarioExiste)
            {
                ModelState.AddModelError(nameof(request.UsuarioResponsavelId), "Usuário responsável não encontrado.");
                return ValidationProblem(ModelState);
            }

            usuarioResponsavelId = request.UsuarioResponsavelId.Value;
        }

        var projeto = new Projeto
        {
            Titulo = request.Titulo.Trim(),
            NumeroEdital = _projetoDomainService.NormalizarNumeroEdital(request.NumeroEdital),
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            ValorTotalOrcamento = request.ValorTotalOrcamento,
            UsuarioResponsavelId = usuarioResponsavelId
        };

        _dbContext.Projetos.Add(projeto);
        _dbContext.SaveChanges();

        var response = _projetoMetricsService.GetProjetoDetalhe(projeto.Id, CurrentUserId, IsAdmin, null);

        return CreatedAtAction(nameof(GetById), new { id = projeto.Id }, response);
    }

    [HttpPut("{id:int}")]
    public ActionResult<ProjetoDetalheResponse> Update(
        int id,
        [FromBody] ProjetoUpsertRequest request)
    {
        if (!ValidateProjetoRequest(request))
        {
            return ValidationProblem(ModelState);
        }

        var projeto = _projetoDomainService.ObterProjeto(id, CurrentUserId, IsAdmin);

        if (projeto is null)
        {
            return NotFound();
        }

        if (IsAdmin && request.UsuarioResponsavelId.HasValue && request.UsuarioResponsavelId.Value != projeto.UsuarioResponsavelId)
        {
            var usuarioExiste = _dbContext.Usuarios
                .Any(x => x.Id == request.UsuarioResponsavelId.Value);

            if (!usuarioExiste)
            {
                ModelState.AddModelError(nameof(request.UsuarioResponsavelId), "Usuário responsável não encontrado.");
                return ValidationProblem(ModelState);
            }

            projeto.UsuarioResponsavelId = request.UsuarioResponsavelId.Value;
        }

        projeto.Titulo = request.Titulo.Trim();
        projeto.NumeroEdital = _projetoDomainService.NormalizarNumeroEdital(request.NumeroEdital);
        projeto.DataInicio = request.DataInicio;
        projeto.DataFim = request.DataFim;
        projeto.ValorTotalOrcamento = request.ValorTotalOrcamento;

        _dbContext.SaveChanges();

        var response = _projetoMetricsService.GetProjetoDetalhe(projeto.Id, CurrentUserId, IsAdmin, null);

        return Ok(response);
    }

    private bool ValidateProjetoRequest(ProjetoUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo) || request.Titulo.Trim().Length < 3)
        {
            ModelState.AddModelError(nameof(request.Titulo), "O título do projeto deve ter pelo menos 3 caracteres.");
        }

        if (request.ValorTotalOrcamento <= 0)
        {
            ModelState.AddModelError(nameof(request.ValorTotalOrcamento), "O valor total do orçamento deve ser positivo.");
        }

        if (request.DataInicio == default)
        {
            ModelState.AddModelError(nameof(request.DataInicio), "A data de início é obrigatória.");
        }

        if (request.DataFim.HasValue && request.DataFim.Value < request.DataInicio)
        {
            ModelState.AddModelError(nameof(request.DataFim), "A data de fim deve ser posterior à data de início.");
        }

        return ModelState.IsValid;
    }
}
