using System.Text.Json;
using Estoque.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(NotificationBroadcastService broadcastService) : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        var subscription = broadcastService.Subscribe();

        try
        {
            await foreach (var message in subscription.reader.ReadAllAsync(cancellationToken))
            {
                await Response.WriteAsync("event: stock\n", cancellationToken);
                await Response.WriteAsync($"data: {JsonSerializer.Serialize(message)}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        finally
        {
            broadcastService.Unsubscribe(subscription.id);
        }
    }
}
