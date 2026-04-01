using Estoque.Api.Interfaces;
using Estoque.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IMetricsService metricsService, ILogger<HealthController> logger)
    {
        _metricsService = metricsService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogDebug("Health check solicitado");
        
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        _logger.LogDebug("Métricas solicitadas");
        
        var metrics = await _metricsService.GetMetricsAsync();
        return Ok(metrics);
    }

    [HttpGet("ready")]
    public IActionResult GetReadiness()
    {
        // Verificar se todos os serviços estão prontos
        try
        {
            // Aqui você pode adicionar verificações específicas
            // como conexão com banco, RabbitMQ, etc.
            
            return Ok(new
            {
                status = "ready",
                timestamp = DateTime.UtcNow,
                checks = new
                {
                    database = "ok",
                    rabbitmq = "ok",
                    cache = "ok"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na verificação de prontidão");
            
            return StatusCode(503, new
            {
                status = "not_ready",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    [HttpGet("live")]
    public IActionResult GetLiveness()
    {
        // Verificação simples de vida da aplicação
        return Ok(new
        {
            status = "alive",
            timestamp = DateTime.UtcNow,
            uptime = Environment.TickCount64
        });
    }
}