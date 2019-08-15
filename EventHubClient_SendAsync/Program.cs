using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;

namespace EventHubClient_SendAsync
{
    class Program
    {
        private static EventHubClient eventHubClient;

        // e.g. Endpoint=sb://consistency.servicebus.windows.net/;SharedAccessKeyName=send_only;SharedAccessKey=ABC123jVJr6tSFDxhgIKNxWKMbPxaEHzgRa3ovZOk=
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

            await SendEventsToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        // Uses the event hub client to send 100 events to the same event hub partition
        private static async Task SendEventsToEventHub(int numEventsToSend)
        {
            for (var i = 0; i < numEventsToSend; i++)
            {
                try
                {
                    var eventMsg = $"Event {i}";
                    Console.WriteLine($"Sending event: {eventMsg}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(eventMsg)),"Device1");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numEventsToSend} events sent.");
        }
    }
}
