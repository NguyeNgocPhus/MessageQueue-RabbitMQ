namespace RabbitMQ.Configuration;

public class RabbitMQConfiguration
{
    public string Queue { get; set; }
    public string HostName { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string RetryCount { get; set; }
    public string ExchangeName { get; set; }
    public string ExchangeType { get; set; }
}