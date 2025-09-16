using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wara.Api.Controllers;

[ApiController]
[Route("")]
public class UiController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public UiController(IWebHostEnvironment env) => _env = env;

    [HttpGet("user")]
    [Authorize]
    public IActionResult UserPage()
    {
        var path = Path.Combine(_env.ContentRootPath, "PrivateUi", "user.html");
        return PhysicalFile(path, "text/html");
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPage()
    {
        var path = Path.Combine(_env.ContentRootPath, "PrivateUi", "admin.html");
        return PhysicalFile(path, "text/html");
    }
}
