using Microsoft.Azure.ServiceBus;

namespace ServiceBusCLI.Utils
{
    public interface IQueueClientAccessor
    {
        QueueClient QueueClient { get; }
    }
}
