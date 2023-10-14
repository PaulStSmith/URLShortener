namespace URLShortener.Model
{
    /// <summary>
    /// Describes a URL and its shortened version.
    /// </summary>
    public interface IShortUrlModel : IEntity
    {
        /// <summary>
        /// Gets the date the URL was created.
        /// </summary>
        DateTime? DateCreated { get; }

        /// <summary>
        /// Gets or sets the number of hits that this URL had.
        /// </summary>
        /// <remarks>
        /// Hits are updated every time a short URL is used to redirect the caller to the actual URL.
        /// </remarks>
        int Hits { get; set; }

        /// <summary>
        /// Gets the shortened version of the <see cref="Url"/>.
        /// </summary>
        string ShortUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL (Uniform Resource Locator).
        /// </summary>
        string Url { get; set; }
    }
}