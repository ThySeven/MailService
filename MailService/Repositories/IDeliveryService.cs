using MailService.Models;
using System.Threading.Tasks;

namespace MailService.Services
{
    public interface IDeliveryService
    {
        Task SendAsync(MailModel mail);
        Task ReceiveAsync(MailModel mail);
    }
}
