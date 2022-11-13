using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.RabbitMQ.Handler;

public class SendEmailEventHandler : IIntegrationEventHandler
{
    public void Handler(object @event)
    {
        Console.WriteLine($"email handler : {@event.ToString()}");
    }
}