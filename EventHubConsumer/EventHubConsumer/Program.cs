using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;

namespace EventHubConsumer
{
    class Program
    {        
        private const string EventHubConnectionString = "Endpoint=sb://exchangenotifyeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=**********************";
        private const string EventHubName = "exchangeeventhub";

        private const string StorageContainerName = "exchangehubcontainer";
        private const string StorageAccountName = "exchangecontainer";
        private const string StorageAccountKey = "****************";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Processor Ayağa Kalkıyor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Event Processor mesaj almaya başlaması için Host Ediliyor.
            await eventProcessorHost.RegisterEventProcessorAsync<Processor>();

            Console.WriteLine("Yeni paket bekleniyor. Durdurmak için Lütfen Bir Tuşa Basın.");
            Console.ReadLine();

            // Disposes of the Event Processor Host    
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        public class Processor : IEventProcessor
        {
            public Task CloseAsync(PartitionContext context, CloseReason reason)
            {
                Console.WriteLine($"Process Kapanıyor. Partition '{context.PartitionId}', Sebep: '{reason}'.");
                return Task.CompletedTask;
            }

            public Task OpenAsync(PartitionContext context)
            {
                Console.WriteLine($"Processor Ayağa Kalkıyor. Partition: '{context.PartitionId}'");
                return Task.CompletedTask;
            }

            public Task ProcessErrorAsync(PartitionContext context, Exception error)
            {
                Console.WriteLine($"Partition'da Hata Var: {context.PartitionId}, Hata: {error.Message}");
                return Task.CompletedTask;
            }

            public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
            {
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    Console.WriteLine($"Message alındı. Partition: '{context.PartitionId}', Kur Değeri: '{data}'");
                }

                return context.CheckpointAsync();
            }
        }
    }
}
