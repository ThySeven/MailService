using NLog;
using ILogger = NLog.ILogger;

namespace MailService.Repositories
{
    public class AuctionCoreLogger
    {
        public static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}