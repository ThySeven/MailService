using MailService.Models;
using System.Threading.Tasks;

namespace MailService.Services
{
    public interface IDeliveryService
    {
        Task SendAsync(AutoMail autoMail);
        Task ReceiveAsync(AutoMail autoMail);
    }
}
