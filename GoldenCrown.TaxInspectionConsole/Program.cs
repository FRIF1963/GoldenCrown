using GoldenCrown.TaxInspectionConsole.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime;
using System.Text;
using System.Text.Json;

namespace GoldenCrown.TaxInspectionConsole
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Tax Inspection Console!");

            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RabbitMQ__Hostname") ?? "localhost",
                UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest",
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var queue = "TransactionCreatedEvent";

            await channel.ExchangeDeclareAsync(queue, ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue, queue, routingKey: "");


            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<TransactionCreatedEvent>(json);

                Console.WriteLine("SenderID: " + @event.SenderId + ", RecieverID: " + @event.ReceiverId + 
                    ", Amount: " + @event.Amount + ", Currency: " + @event.Currency);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue, autoAck: false, consumer);

            Console.WriteLine("Listening for messages...");
            await Task.Delay(Timeout.Infinite);
        }


    }
}
