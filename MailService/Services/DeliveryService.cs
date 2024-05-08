using MailService.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace MailService.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly EmailSender _emailSender;
        private readonly EmailReceiver _emailReceiver;

        public DeliveryService()
        {
            _emailSender = new EmailSender();
            _emailReceiver = new EmailReceiver();
        }

        public async Task SendAsync(AutoMail autoMail)
        {
            // Send the mail model via email
            await _emailSender.SendMailAsync(autoMail);
        }

        public async Task ReceiveAsync(AutoMail autoMail)
        {
            _emailReceiver.ListenAndSendToRabbitMQ();
        }
    }
}
