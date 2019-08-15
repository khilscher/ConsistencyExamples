using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PartitionReceiver_ReceiveAsync
{
    class Program
    {
        private static EventHubClient eventHubClient;
        private static PartitionReceiver receiver;

        // e.g. Endpoint=sb://consistency.servicebus.windows.net/;SharedAccessKeyName=receive_only;SharedAccessKey=ABC123jVJr6tSFDxhgIKNxWKMbPxaEHzgRa3ovZOk=
        private const string EventHubConnectionString = "{Your EH connection string}";
        private const string EventHubName = "{Your EH name}";

        static async Task Main(string[] args)
        {

            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            Task.Run(() => ReceiveMessagesFromPartition("2", ct),ct);

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
            tokenSource.Cancel();

            await eventHubClient.CloseAsync();

        }

        private static async Task ReceiveMessagesFromPartition(string partition, CancellationToken ct)
        {

            receiver = eventHubClient.CreateReceiver(PartitionReceiver.DefaultConsumerGroupName, partition, EventPosition.FromEnqueuedTime(DateTime.Now));

            Console.WriteLine($"Connected to partition {receiver.PartitionId}, consumer group {receiver.ConsumerGroupName}");

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                // Receive a maximum of 100 messages in this call to ReceiveAsync
                var ehEvents = await receiver.ReceiveAsync(100);

                // ReceiveAsync can return null if there are no messages
                if (ehEvents != null)
                {
                    // Since ReceiveAsync can return more than a single event you will need a loop to process
                    foreach (var ehEvent in ehEvents)
                    {
                        // Decode the byte array segment
                        var message = UnicodeEncoding.UTF8.GetString(ehEvent.Body.Array);
                        Console.WriteLine($"Received. '{message}'");
                    }
                }
            }
        }
    }
}
