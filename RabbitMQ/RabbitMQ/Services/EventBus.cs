using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using RabbitMQ.Configuration;
using RabbitMQ.Events;
using RabbitMQ.RabbitMQ.Event;
using RabbitMQ.RabbitMQ.Interfaces;

namespace RabbitMQ.RabbitMQ.Services;

public class EventBus : IEventBus, IDisposable
{
    private readonly RabbitMQConfiguration _rabbitMqConfiguration;
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public EventBus(IOptions<RabbitMQConfiguration> options)
    {
        _rabbitMqConfiguration = options.Value;
        _connection = CreateConnection();
        _channel = CreateChannel();

    }

    private IConnection CreateConnection()
    {
        var uri =
            $"amqp://{_rabbitMqConfiguration.UserName}:{_rabbitMqConfiguration.Password}@{_rabbitMqConfiguration.HostName}:{_rabbitMqConfiguration.Port}/";

        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri)
        };
        // 1. create connection
        var connection = factory.CreateConnection();
        return connection;
    }

    private IModel CreateChannel()
    {
        // 2. create channel
        var channel = _connection.CreateModel();
        // 3. create exchange
        channel.ExchangeDeclare(_rabbitMqConfiguration.ExchangeName, _rabbitMqConfiguration.ExchangeType);

        return channel;
        
    }

    public void SubscribeDynamic(ChannelWriter<MessageBase> writer)
    {
        var random = new Random();
        var branchId = $"branch_{random.Next(1, 100)}";
        var customerId = $"customer-{random.Next(1, 100)}";

        var queueName = $"queue-customerId-{customerId}";
        // 4. create queue
        var arguments = new Dictionary<string, object>
        {
            // { "x-message-ttl", 60000 },  // Thời gian sống tối đa của tin nhắn trong hàng đợi (TTL) là 60 giây
            // { "x-expires", 300000 },     // Hàng đợi sẽ bị xóa sau 5 phút nếu không hoạt động
            // { "x-max-length", 1000 },    // Giới hạn số lượng tin nhắn tối đa là 1000
            // { "x-max-length-bytes", 10485760 }, // Giới hạn dung lượng tối đa của hàng đợi là 10 MB
            // { "x-dead-letter-exchange", "dlx-exchange" },  // Dead letter exchange để chuyển tiếp tin nhắn bị từ chối
            { "x-single-active-consumer", true }, // Routing key cho dead letter exchange
        };
        _channel.QueueDeclare(queueName, false, true, true, arguments);
        //5. bind exchange to queue with routingKey

        _channel.QueueBind(queueName, _rabbitMqConfiguration.ExchangeName, Const.Broadcast_To_All);
        _channel.QueueBind(queueName, _rabbitMqConfiguration.ExchangeName,
            string.Format(Const.Broadcast_To_Branch, branchId));
        _channel.QueueBind(queueName, _rabbitMqConfiguration.ExchangeName,
            string.Format(Const.Broadcast_To_Customer, customerId));
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, eventArgs) =>
        {
            var eventName = eventArgs.RoutingKey;
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageBase = new MessageBase()
            {
                Message = message,
            };
            writer.WriteAsync(messageBase);
        };
        _channel.BasicConsume(queueName, true, consumer);
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;
        string jsonString = JsonSerializer.Serialize(@event);

        var body = Encoding.UTF8.GetBytes(jsonString);
        _channel.BasicPublish(exchange: _rabbitMqConfiguration.ExchangeName, routingKey: eventName, body: body);
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
        _channel.Close();
        _channel?.Dispose();
        Console.WriteLine("RabbitMQ connection closed.");
    }
}