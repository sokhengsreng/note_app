using Microsoft.AspNetCore.Mvc;

namespace NotesApplication.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Detailed health check
    /// </summary>
    /// <returns>Detailed health information</returns>
    [HttpGet("healthz")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthDetails()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Notes API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            timestamp = DateTime.UtcNow
        });
    }
}
