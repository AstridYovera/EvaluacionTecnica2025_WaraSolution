using Microsoft.AspNetCore.Mvc;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Controllers.Test;

[ApiController]
[Route("api/test-email")]
public class TestEmailController : ControllerBase
{
    private readonly IEmailService _email;

    public TestEmailController(IEmailService email)
    {
        _email = email;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromQuery] string to)
    {
        try
        {
            await _email.SendAsync(
                to,
                "Prueba de correo WARA",
                "<h3>¡Hola Astrid!</h3><p>Este es un correo de prueba enviado desde tu API.</p>"
            );
            return Ok(new { message = "Correo enviado correctamente a " + to });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
