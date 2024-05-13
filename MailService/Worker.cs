using MailService.Models;
using MailService.Repositories;
using MailService.Services;
using NLog.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MailService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DeliveryService _deliveryService;
        private readonly EmailReceiver _emailReceiver;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _deliveryService = new DeliveryService();
            _emailReceiver = new EmailReceiver();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("RabbitMQHostName") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: Environment.GetEnvironmentVariable("RabbitMQQueueName"),
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var mail = new EventingBasicConsumer(channel);
            mail.Received += async (model, ea) =>
            {
                var mail = ea.Body.ToArray();
                var uftString = Encoding.UTF8.GetString(mail);
                var message = JsonSerializer.Deserialize<MailModel>(uftString);

                await _deliveryService.SendAsync(message);
            };

            channel.BasicConsume(queue: Environment.GetEnvironmentVariable("RabbitMQQueueName"),
                                 autoAck: true,
                                 consumer: mail);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Run the email receiver method every 5 seconds
                await Task.Run(() => _emailReceiver.ListenAndSendToRabbitMQ());
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
