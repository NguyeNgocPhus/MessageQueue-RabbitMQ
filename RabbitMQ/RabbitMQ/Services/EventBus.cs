using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Configuration;
using RabbitMQ.Events;
using RabbitMQ.RabbitMQ.Event;
using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.RabbitMQ.Services;

public class EventBus : IEventBus
{
    private readonly RabbitMQConfiguration _rabbitMqConfiguration;
    private readonly IModel _channel;
    private Dictionary<string ,Type> _subscriptions { get; set; } = new Dictionary<string, Type>();

    public EventBus(IOptions<RabbitMQConfiguration> options)
    {
        _rabbitMqConfiguration = options.Value;
        _channel = CreateChannel();
        Console.WriteLine("helloi");
    }

    public IModel CreateChannel()
    {
        var uri = $"amqp://{_rabbitMqConfiguration.UserName}:{_rabbitMqConfiguration.Password}@{_rabbitMqConfiguration.HostName}:{_rabbitMqConfiguration.Port}/";

        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri)
        };
        // 1. create connection
        var connection = factory.CreateConnection();
        // 2. create channel
        var channel = connection.CreateModel();
        // 3. create exchange
        channel.ExchangeDeclare(_rabbitMqConfiguration.ExchangeName, _rabbitMqConfiguration.ExchangeType);
        // 4. create queue
        channel.QueueDeclare(_rabbitMqConfiguration.Queue, true, false, false, null);
        return channel;
    }

    public void SubscribeDynamic<IHandler>(string routingKey)
    {
        //5. bind exchange to queue with routingKey
        _channel.QueueBind(_rabbitMqConfiguration.Queue, _rabbitMqConfiguration.ExchangeName, routingKey);
        _subscriptions.Add(routingKey, typeof(IHandler));
        StartBasicConsumer();
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;
        string jsonString = JsonSerializer.Serialize(@event);

        var body = Encoding.UTF8.GetBytes(jsonString);
        _channel.BasicPublish(exchange: _rabbitMqConfiguration.ExchangeName, routingKey: eventName, body: body);
    }

    private void StartBasicConsumer()
    {
        Console.WriteLine("Starting RabbitMQ basic consume");
        if (_channel != null)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(_rabbitMqConfiguration.Queue, false, consumer);
        }
    }

    public void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(_subscriptions[eventName] + ""+message);
    }
}