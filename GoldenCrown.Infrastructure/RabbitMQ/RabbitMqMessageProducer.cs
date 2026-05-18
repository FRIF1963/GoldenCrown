using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.RabbitMQ
{
    public class RabbitMqMessageProducer : IMessageProducer
    {
        public RabbitMqSettings _settings;

        public RabbitMqMessageProducer(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendMessageAsync<T>(T message, CancellationToken token = default)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Hostname,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            await using var connection = await factory.CreateConnectionAsync(token);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: token);

            var queue = typeof(T).Name;

            await channel.ExchangeDeclareAsync(queue, ExchangeType.Direct, cancellationToken: token);
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: token);
            await channel.QueueBindAsync(queue, queue, routingKey: queue, null, cancellationToken: token);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            
            await channel.BasicPublishAsync("", queue, body, cancellationToken: token);
        }

        
    }
}
