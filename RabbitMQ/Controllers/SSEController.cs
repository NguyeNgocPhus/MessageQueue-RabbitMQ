using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Events;
using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.Controllers;

[ApiController]
public class SSEController : ControllerBase
{
    private readonly ILogger<SSEController> _logger;
    private readonly IEventBus _eventBus;

    public SSEController(ILogger<SSEController> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    [HttpGet]
    [Route("event-stream")]
    public async Task Get()
    {
        var cancellationToken = HttpContext.RequestAborted;
        try
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            var channel = Channel.CreateUnbounded<MessageBase>();
            var reader = channel.Reader;
            var writer = channel.Writer;
            await Response.WriteAsync($"Hallo", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            _eventBus.SubscribeDynamic(writer);
            await foreach (var coordinates in reader.ReadAllAsync(cancellationToken))
            {
                // Gửi một sự kiện SSE
                await Response.WriteAsync($"data: {coordinates.Message}", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Client đã ngắt kết nối!");
        }
    }
}