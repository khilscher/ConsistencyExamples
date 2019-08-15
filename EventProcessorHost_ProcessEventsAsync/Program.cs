using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;

namespace EventProcessorHost_Receive_From_All_Partitions
{
    class Program
    {
        // e.g. Endpoint=sb://consistency.servicebus.windows.net/;SharedAccessKeyName=send_only;SharedAccessKey=ABC123jVJr6tSFDxhgIKNxWKMbPxaEHzgRa3ovZOk=
        private const string EventHubConnectionString = "{Your EH connection string}";
        private const string EventHubName = "{Your EH name}";

        // e.g. ABC123jkoYCjOJBl3JK1sHM9h3wbXnvmiK/s5eVP8amcHpRehK+tt/GUL6IkJeNEw5MDh8JIXDbLDp7UvjhAX/yw==
        private const string StorageAccountKey = "{Your storage account key}";
        private const string StorageAccountName = "{Your storage account name}";
        private const string StorageContainerName = "{Your storage account container name}";
  
        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
