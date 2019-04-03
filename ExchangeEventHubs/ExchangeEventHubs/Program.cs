using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ExchangeEventHubs
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateRandomMessages();
        }
        public static void GenerateRandomMessages()
        {
            var primaryConnectionString = "Endpoint=sb://exchangenotifyeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=***************************";
            var client = EventHubClient.CreateFromConnectionString(primaryConnectionString, "exchangeeventhub");
            Random randomNumberGenerator = new Random();

            int counter = 100;
            while (true)
            {
                try
                {
                    counter++;
                    // generate any random numbet between 1 and 1000  
                    var randomMessage = string.Format("Son Güncel Kur {0}", 5.4 + (double)(randomNumberGenerator.Next(100, 1000))/1000);

                    Console.WriteLine("Generated message: {0}", randomMessage);

                    client.Send(new EventData(Encoding.UTF8.GetBytes(randomMessage)));
                    if(counter>110) { return; }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Thread.Sleep(1000);                
            }
            Console.ReadLine();
        }
    }
}
