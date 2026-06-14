using finansee_api.DTOs;
using finansee_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finansee_api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController(IProjetoMetricsService projetoMetricsService) : ApiControllerBase
{
    private readonly IProjetoMetricsService _projetoMetricsService = projetoMetricsService;

    [HttpGet]
    public ActionResult<DashboardResponse> Get()
    {
        var response = _projetoMetricsService.GetDashboard(CurrentUserId, IsAdmin);

        return Ok(response);
    }
}
