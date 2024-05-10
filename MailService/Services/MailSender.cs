using System.Net.Mail;
using System.Net;
using System.Text.Json;
using MailService.Models;
using System.Text;

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
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}