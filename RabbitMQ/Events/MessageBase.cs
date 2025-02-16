using RabbitMQ.RabbitMQ.Event;

namespace RabbitMQ.Events;

public class MessageBase : IntegrationEvent
{
    public string Name { get; set; }
    public string Message { get;set; }
}