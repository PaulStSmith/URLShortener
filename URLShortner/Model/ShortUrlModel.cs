﻿using NHibernate.Mapping.Attributes;
using System.Text;
using URLShortener.Common.Model;

namespace URLShortener.Model
{
    /// <summary>
    /// Represents a URL and its shortened version.
    /// </summary>
    public class ShortUrlModel : ItemModelBase, IShortUrlModel
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
                _shortUrlLength ??= Program.App.Configuration.GetValue<int>("shortUrlLength", 6);

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

        /// <summary>
        /// Returns a clone of thins instance.
        /// </summary>
        /// <returns>A clone of thins instance.</returns>
        public override object Clone()
        {
            return new ShortUrlModel() {
                Id = this.Id,
                Url = this.Url,
                Hits = this.Hits,
                ShortUrl = this.ShortUrl,
                DateCreated = this.DateCreated,
            };
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public virtual bool Equals(IShortUrlModel? other)
        {
            if (this.Id != other?.Id) return false;
            if (this.Url!= other?.Url) return false;
            if (this.Hits != other?.Hits) return false;
            if (this.ShortUrl != other?.ShortUrl) return false;
            if (this.DateCreated != other?.DateCreated) return false;
            return true;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public override bool Equals(object? other) => this.Equals((IShortUrlModel?)other);

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => this.ShortUrl.GetHashCode();

        /// <summary>
        /// Implicitly converts <see cref="ShortUrlModel"/> to <see cref="ShortUrlDTO"/>.
        /// </summary>
        /// <param name="model">The <see cref="ShortUrlModel"/> to be converted to <see cref="ShortUrlDTO"/>.</param>
        public static implicit operator ShortUrlDTO(ShortUrlModel model)
        {
            return new ShortUrlDTO
            {
                Id = model.Id.ToString(),
                Url = model.Url,
                Hits = model.Hits,
                ShortUrl = Path.Combine(Program.ShortUrlBase, model.ShortUrl),
                DateCreated = model.DateCreated,
            };
        }

        /// <summary>
        /// Implicitly converts <see cref="ShortUrlDTO"/> to <see cref="ShortUrlModel"/>.
        /// </summary>
        /// <param name="dto">The <see cref="ShortUrlDTO"/> to be converted to <see cref="ShortUrlModel"/>.</param>
        public static implicit operator ShortUrlModel(ShortUrlDTO dto)
        {
            return new ShortUrlModel() 
            { 
                Id = int.Parse(dto.Id),
                Url = dto.Url,
                Hits = dto.Hits,
                ShortUrl = Path.GetFileName(dto.ShortUrl),
                DateCreated = dto.DateCreated,
            };
        }
    }
}
