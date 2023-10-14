﻿using NHibernate.Mapping.Attributes;
using System.Text;

namespace URLShortener.Model
{
    /// <summary>
    /// Represents a URL and its shortened version.
    /// </summary>
    public class ShortUrlModel : IShortUrlModel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShortUrlModel() { }

        /// <summary>
        /// Creates a new <see cref="ShortUrl"/> with the specified <paramref name="url"/> and <paramref name="shortUrl"/>.
        /// </summary>
        /// <param name="url">The long URL.</param>
        /// <param name="shortUrl">The short URL.</param>
        public ShortUrlModel(string url, string shortUrl)
        {
            this.Url = url;
            this.ShortUrl = shortUrl;
        }

        /// <summary>
        /// Creates a new <see cref="ShortUrl"/> with the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The long URL to be shortened.</param>
        public ShortUrlModel(string url) : this(url, GenerateShortUrl()) { }

        /// <summary>
        /// Gets or sets an identifier for the URL.
        /// </summary>
        public virtual int Id { get; protected internal set; }

        /// <summary>
        /// Gets or sets the URL (Uniform Resource Locator).
        /// </summary>
        public virtual string Url { get; set; } = "";

        /// <summary>
        /// Gets or sets the shortened version of the <see cref="Url"/>.
        /// </summary>
        public virtual string ShortUrl { get; set; } = "";

        /// <summary>
        /// Gets or sets the number of hits that this URL had.
        /// </summary>
        /// <remarks>
        /// Hits are updated every time a short URL is used to redirect the caller to the actual URL.
        /// </remarks>
        public virtual int Hits { get; set; }

        /// <summary>
        /// Gets or sets the date the URL was created.
        /// </summary>
        public virtual DateTime? DateCreated { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the length of the short URL.
        /// </summary>
        public static int ShortUrlLength
        {
            get
            {
                if (_shortUrlLength == null)
                    _shortUrlLength = Program.App.Configuration.GetValue<int>("shortUrlLength", 6);

                return (int)_shortUrlLength;
            }
        }
        private static int? _shortUrlLength;

        /// <summary>
        /// Generates a short URL.
        /// </summary>
        /// <returns>A short URL.</returns>
        private static string GenerateShortUrl()
        {
            var sb = new StringBuilder();
            var rnd = new Random();
            for (var i = 0; i < ShortUrlLength; i++)
                sb.Append(Alphabet[rnd.Next(0, Alphabet.Length)]);
            return sb.ToString();
        }
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    }
}
