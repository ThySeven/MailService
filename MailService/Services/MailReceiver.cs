using System;
using System.Net;
using System.Net.Mail;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using MailService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using MailService.Repositories;

public class EmailReceiver
{

    public EmailReceiver()
    {

    }

    public void ListenAndSendToRabbitMQ()
    {
        using (var client = new ImapClient())
        {
            // Connect to the IMAP server
            client.Connect("imap.gmail.com", 993, true);

            // Authenticate with the server
            client.Authenticate("gronogolsen@gmail.com", "xvgu reqj vpod mbms");

            // Select the inbox folder
            client.Inbox.Open(FolderAccess.ReadWrite);

            // Search for unread messages
            var uids = client.Inbox.Search(SearchQuery.Not(SearchQuery.HasGMailLabel("Received").Or(SearchQuery.SubjectContains("RE:").Or(SearchQuery.SubjectContains("SV:").Or(SearchQuery.SubjectContains("VS:").Or(SearchQuery.SubjectContains("FWD:")))))));


            foreach (var uid in uids)
            {
                // Fetch the message
                var message = client.Inbox.GetMessage(uid);

                // Process the message
                ProcessEmailMessage(message);

                // Mark the message as "Seen"
                client.Inbox.SetLabels(uid, new List<string> { "Received" }, true);
                AuctionCoreLogger.Logger.Info($"Email received from {message.From}");
            }

            // Disconnect from the server
            client.Disconnect(true);
        }
    }

    private void ProcessEmailMessage(MimeMessage message)
    {
        try
        {
            // Extract the necessary information from the email message
            string senderMail = message.From.ToString();
            string receiverMail = message.To.ToString();
            string subject = message.Subject;
            string body = message.TextBody;

            // Create an instance of mail
            var mail = new MailModel
            {
                ReceiverMail = senderMail,
                Header = "Tak for din henvendelse",
                Content = File.ReadAllText("Models/Compliments.html"),
            };

            // Send the email to RabbitMQ
            SendToRabbitMQ(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing email: {ex.Message}");
        }
    }

    private void SendToRabbitMQ(MailModel mail)
    {
        try
        {
            var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("RabbitMQHostName") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Serialize the received email data
            var serializedEmail = JsonSerializer.Serialize(mail);

            // Publish the serialized email data to RabbitMQ
            channel.QueueDeclare(queue: Environment.GetEnvironmentVariable("RabbitMQQueueName"),
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

            channel.BasicPublish(exchange: "",
                                          routingKey: Environment.GetEnvironmentVariable("RabbitMQQueueName"),
                                          basicProperties: null,
                                          body: Encoding.UTF8.GetBytes(serializedEmail));

            Console.WriteLine("Sent email to RabbitMQ: " + serializedEmail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email to RabbitMQ: {ex.Message}");
        }
    }
}
