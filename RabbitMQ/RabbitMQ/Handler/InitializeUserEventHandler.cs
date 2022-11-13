using RabbitMQ.RabbitMQ.Event;
using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.RabbitMQ.Handler;

public class InitializeUserEventHandler : IIntegrationEventHandler
{
    public void Handler(object @event)
    {
        Console.WriteLine($"initialize user handler : {@event.ToString()}");
    }
}