using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using URLShortener.Common.Model;
using URLShortenerEventConsumer;

namespace URLShortnerEventConsumer
{
    internal class Program
    {
        /// <summary>
        /// Gets or sets an access to the program configuration.
        /// </summary>
        private static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the URL for the URL message hub.
        /// </summary>
        public static string MsgHubUrl => Configuration["msgHubUrl"] ?? "";

        /// <summary>
        /// Type Constructor.
        /// </summary>
        static Program()
        {
            // Create a ConfigurationBuilder
            var builder = new ConfigurationBuilder()
                                .SetBasePath(AppContext.BaseDirectory)
                                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets or sets the hub connection.
        /// </summary>
        private static HubConnection Connection { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        static void Main()
        {
            Console.WriteLine(AppResources.Banner);
            WaitAPI();
            Console.WriteLine();
            ConnectToHub();

            while (Console.ReadKey().Key != ConsoleKey.Q) 
                Thread.Sleep(1);
        }

        /// <summary>
        /// Waits 20 seconds for the API to come up.
        /// </summary>
        private static void WaitAPI()
        {
            var nSec = 20;
            var tmr = new System.Timers.Timer(1000);
            tmr.Elapsed += (s, e) => { Console.Write($"Awaiting API service to start ({nSec--}).   \r"); };
            Console.WriteLine();
            tmr.Start();
            while(nSec > 0)
                Thread.Sleep(1);
            tmr.Stop();
            Console.WriteLine(new string(' ', 40));
        }

        /// <summary>
        /// Connects to the message hub.
        /// </summary>
        private static void ConnectToHub()
        {
            Connection = new HubConnectionBuilder().WithUrl(MsgHubUrl).Build();
            Connection.On<string>("ItemAdded", ItemAdded);
            Connection.On<string, string>("ItemUpdated", ItemUpdated);
            Connection.On<string>("ItemDeleted", ItemDeleted);

            Connection.StartAsync();
        }

        /// <summary>
        /// Processes the “ItemDeleted” message.
        /// </summary>
        /// <param name="jsonItem">The deleted item, in JSON format.</param>
        private static void ItemDeleted(string jsonItem)
        {
            var item = JsonSerializer.Deserialize<ShortUrlDTO>(jsonItem);
            Console.WriteLine($@"Short URL “{item?.ShortUrl}” was deleted.");
        }

        /// <summary>
        /// Processes the “ItemUpdated” message.
        /// </summary>
        /// <param name="jsonItem1">The old valule of the item, in JSON format.</param>
        /// <param name="jsonItem2">The new valule of the item, in JSON format.</param>
        private static void ItemUpdated(string jsonItem1, string jsonItem2)
        {
            var item1 = JsonSerializer.Deserialize<ShortUrlDTO>(jsonItem1);
            // var item2 = JsonSerializer.Deserialize<ShortUrlDTO>(jsonItem2);
            Console.WriteLine($@"Short URL with id = “{item1?.Id}” was updated.");
        }

        /// <summary>
        /// Processes the “ItemAdded” message.
        /// </summary>
        /// <param name="jsonItem">The added item, in JSON format.</param>
        private static void ItemAdded(string jsonItem)
        {
            var item = JsonSerializer.Deserialize<ShortUrlDTO>(jsonItem);
            Console.WriteLine($@"Short URL “{item?.ShortUrl}” was added.");
        }
    }
}