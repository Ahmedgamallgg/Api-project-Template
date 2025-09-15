global using Microsoft.AspNetCore.Mvc;
global using ServicesAbstractions;
global using Shared;
using System.Security.Claims;
namespace Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public abstract class APIController : ControllerBase
{
    protected string GetEmail() => User.FindFirstValue(ClaimTypes.Email)!;
    protected string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    protected List<string> GetRoles() => User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList()!;
    protected bool IsInRole(string role) => User.IsInRole(role);
    protected string GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    protected int GetTenantId() => int.TryParse(User.FindFirstValue("tenantId")!, out int tenantId) ? tenantId : 0;
}
