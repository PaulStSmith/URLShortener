namespace URLShortener.Model
{
    /// <summary>
    /// Data Transfer Object for <see cref="ShortUrlModel"/>
    /// </summary>
    public class ShortUrlDTO : IShortUrlModel
    {
        public DateTime? DateCreated { get; set; }

        public int Hits { get; set; }

        public string ShortUrl { get; set; }

        public string Url { get; set; }

        public int Id { get; set; }
    }
}
