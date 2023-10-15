using System.Text.Json;
using System.Text.Json.Serialization;

namespace URLShortener.Common.Model
{
    /// <summary>
    /// Data Transfer Object for <see cref="ShortUrlModel"/>.
    /// </summary>
    public class ShortUrlDTO
    {
        /// <summary>
        /// Gets or sets the date the URL was created.
        /// </summary>
        [JsonPropertyName("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the number of hits that this URL had.
        /// </summary>
        /// <remarks>
        /// Hits are updated every time a short URL is used to redirect the caller to the actual URL.
        /// </remarks>
        [JsonPropertyName("hits")]
        public int Hits { get; set; }

        /// <summary>
        /// Gets or sets the shortened version of the <see cref="Url"/>.
        /// </summary>
        [JsonPropertyName("shortUrl")]
        public string ShortUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL (Uniform Resource Locator).
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets an identifier for the URL.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
