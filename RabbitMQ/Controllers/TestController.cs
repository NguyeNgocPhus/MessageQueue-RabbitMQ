using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Events;
using RabbitMQ.Input;
using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly IEventBus _eventBus;

    public TestController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    [HttpPost]
    public IActionResult SendMs1(Test1Input input)
    {
        var @event = new InitializeUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            Email = input.Email

        };
        _eventBus.Publish(@event);
        return Ok("Done send ms 11111111");
    }
    public IActionResult SendMs2(Test2Input input)
    {
        var @event = new SendEmailEvent()
        {
            Id = Guid.NewGuid(),
            Email = input.Email

        };
        _eventBus.Publish(@event);
        return Ok("Done send ms 222222222");
    }
}