using RabbitMQ.RabbitMQ.Event;

namespace RabbitMQ.Events;

public class SendEmailEvent : IntegrationEvent
{
    public string Email { get; set; }
}