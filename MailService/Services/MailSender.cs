using System.Net.Mail;
using System.Net;
using System.Text.Json;
using MailService.Models;

public class EmailSender
{
    private readonly SmtpClient _smtpClient;

    public EmailSender()
    {
        // Initialize SMTP client with Gmail credentials
        _smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("gronogolsen@gmail.com", "vaske123"),
            EnableSsl = true
        };
    }

    public async Task SendMailAsync(AutoMail autoMail)
    {
        try
        {
            // Convert MailModel to JSON string
            var jsonString = JsonSerializer.Serialize(autoMail);

            // Create a new email message
            var message = new MailMessage("gronogolsen@gmail.com", autoMail.ReceiverMail)
            {
                Subject = "New Mail",
                Body = jsonString,
                IsBodyHtml = false
            };

            // Send the email
            await _smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}