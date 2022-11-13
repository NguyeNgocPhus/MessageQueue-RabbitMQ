using RabbitMQ.RabbitMQ.Event;

namespace RabbitMQ.RabbitMQ.Interfaces;

public interface IIntegrationEventHandler
{
    public void Handler(object @event);
}