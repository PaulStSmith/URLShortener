using Microsoft.Extensions.Configuration;

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
        private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the URL for the URL Shortner API.
        /// </summary>
        public string UrlShortnerApiUrl => Configuration["apiUrl"] ?? "";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public  Program() 
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
        }
    }
}