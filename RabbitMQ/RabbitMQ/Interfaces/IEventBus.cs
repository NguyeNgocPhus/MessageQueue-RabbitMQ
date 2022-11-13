using RabbitMQ.RabbitMQ.Event;

namespace RabbitMQ.RabbitMQ.Interfaces;

public interface IEventBus
{
    public void Publish(IntegrationEvent @event);
    public void SubscribeDynamic<IHandler>(string routingKey);
}