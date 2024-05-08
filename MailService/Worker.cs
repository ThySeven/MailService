using MailService.Models;
using MailService.Services;
using NLog.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MailService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private DeliveryService _deliveryService;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _deliveryService = new DeliveryService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("RabbitMQHostName") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "MailQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var mail = new EventingBasicConsumer(channel);
            mail.Received += async (model, ea) =>
            {
                var mail = ea.Body.ToArray();
                var uftString = Encoding.UTF8.GetString(mail);
                var message = JsonSerializer.Deserialize<AutoMail>(uftString);
                Console.WriteLine($" [x] Received {message}");

                await _deliveryService.SendAsync(message);
            };

            channel.BasicConsume(queue: "MailQueue",
                                 autoAck: true,
                                 consumer: mail);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}