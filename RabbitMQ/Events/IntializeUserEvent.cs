using RabbitMQ.RabbitMQ.Event;

namespace RabbitMQ.Events;

public class InitializeUserEvent : IntegrationEvent
{
    public string Name { get; set; }
    public string Email { get; set; }
}