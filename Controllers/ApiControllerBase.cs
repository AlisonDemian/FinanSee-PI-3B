using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finansee_api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected bool IsAdmin =>
        string.Equals(
            User.FindFirstValue(ClaimTypes.Role),
            "admin",
            StringComparison.OrdinalIgnoreCase);
}
