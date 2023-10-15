using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

#pragma warning disable CS8603 // Possible null reference return.

namespace URLShortnerValidator
{
    /// <summary>
    /// Main class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Gets or sets an access to the program configuration.
        /// </summary>
        private static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the URL for the URL Shortner API.
        /// </summary>
        public static string UrlShortnerApiUrl => Configuration["apiUrl"] ?? "";

        /// <summary>
        /// Default Constructor.
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
        /// Program entry point.
        /// </summary>
        static void Main()
        {
            Console.WriteLine(AppResources.Banner);
            Console.WriteLine("Awaiting API service to start.");
            Console.WriteLine();

            Thread.Sleep(new TimeSpan(0, 0, 20));

            if (string.IsNullOrEmpty(UrlShortnerApiUrl))
            {
                Console.WriteLine("Could not locate the URL Shortner API URL.");
                return;
            }

            try
            {
                Console.WriteLine("Short Url  Long URL                                                     Code");
                Console.WriteLine("---------- ------------------------------------------------------------ -------");
;
                foreach (var item in GetList()) 
                {
                    Console.Write(Path.GetFileName(item.ShortUrl).PadRight(11));
                    Console.Write(ShortenPath(item.Url, 60).PadRight(61));
                    Console.WriteLine((ValidateShortUrl(item.ShortUrl)).PadRight(6));
                }
                Console.WriteLine("---------- ------------------------------------------------------------ -------");
                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("An error has occurred.");
                Console.WriteLine($@"{{{e.GetType}}}");
                Console.WriteLine($@"{e.Message}");
            }
        }

        /// <summary>
        /// Validates the specified short URL.
        /// </summary>
        /// <param name="shortUrl">The short URL to validate.</param>
        /// <returns>The status of the short URL.</returns>
        private static string ValidateShortUrl(string shortUrl)
        {
            using var cli = new HttpClient();
            var p = Path.Combine(UrlShortnerApiUrl, $@"Validate/{WebUtility.UrlEncode(shortUrl)}");
            var resp = cli.GetAsync(p).GetAwaiter().GetResult();
            return ((int)resp.StatusCode).ToString();
        }

        /// <summary>
        /// Shortens a long path for display.
        /// </summary>
        /// <param name="path">The path to be shortened.</param>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns>The shortened path for display</returns>
        private static string ShortenPath(string path, int maxSize)
        {
            if (path.Length < maxSize) 
                return path;

            var l = Math.Min(path.Length / 2, maxSize / 2) - 3;
            return path[..l] + "..." + path[(path.Length - l)..];
        }

        /// <summary>
        /// Returns a list of short URLs from the API.
        /// </summary>
        /// <returns></returns>
        static IList<ShortUrlDTO> GetList()
        {
            using var cli = new HttpClient();
            var p = Path.Combine(UrlShortnerApiUrl, "List");
            var resp = cli.GetAsync(p).GetAwaiter().GetResult();
            if (resp != null && resp.IsSuccessStatusCode) 
            {
                var json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonSerializer.Deserialize<List<ShortUrlDTO>>(json);
            }
            return null;
        }
    }
}