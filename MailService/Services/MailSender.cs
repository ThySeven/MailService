﻿using System.Net.Mail;
using System.Net;
using System.Text.Json;
using MailService.Models;
using System.Text;
using MailService.Repositories;

public class EmailSender
{
    private readonly SmtpClient _smtpClient;

    public EmailSender()
    {
        // Initialize SMTP client with Gmail credentials
        _smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("gronogolsen@gmail.com", "xvgu reqj vpod mbms"),
            EnableSsl = true
        };
    }

    public async Task SendMailAsync(MailModel mail)
    {
        bool sent = false;
        while (!sent)
        {
            try
            {
                // Convert MailModel to JSON string
                var jsonString = JsonSerializer.Serialize(mail);

                // Create a new email message
                var message = new MailMessage("gronogolsen@gmail.com", mail.ReceiverMail)
                {
                    Subject = mail.Header,
                    Body = mail.Content,
                    IsBodyHtml = true
                };

                // Send the email
                await _smtpClient.SendMailAsync(message);
                AuctionCoreLogger.Logger.Info($"Email sent to {mail.ReceiverMail}");
                sent = true;
            }
            catch (Exception ex)
            {
                sent = false;
                // Handle any exceptions
                AuctionCoreLogger.Logger.Error($"Failed to send email to {mail.ReceiverMail}\n {ex.Message}");
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}