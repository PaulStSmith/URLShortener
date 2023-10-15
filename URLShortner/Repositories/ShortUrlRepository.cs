using URLShortener.Model;

namespace URLShortener.Repositories
{
    /// <summary>
    /// Represents a repository for <see cref="ShortUrlModel"/>.
    /// </summary>
    public class ShortUrlRepository : Repository<ShortUrlModel>
    {
        /// <summary>
        /// Adds a new <see cref="ShortUrlModel"/> to the repository with the specified <paramref name="url"/>, creating the short URL.
        /// </summary>
        /// <param name="url">The long URL that the short URL will point to.</param>
        /// <returns>The newly added <see cref="ShortUrlModel"/>.</returns>
        public ShortUrlModel Add(string url)
        {
            var model = this.GetByUrl(url);
            if (model == null)
            {
                model = new ShortUrlModel(url);
                base.Add(model);
            }
            return model;
        }

        /// <summary>
        /// Gets a <see cref="ShortUrlModel"/> for the specified <paramref name="shortUrl"/>.
        /// </summary>
        /// <param name="shortUrl">A short URL.</param>
        /// <returns>A <see cref="ShortUrlModel"/> for the specified <paramref name="shortUrl"/>.</returns>
        public ShortUrlModel? GetByShortUrl(string shortUrl) 
        {
            return QueryDB().Where(x => x.ShortUrl == shortUrl).FirstOrDefault();
        }

        /// <summary>
        /// Gets a <see cref="ShortUrlModel"/> for the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">A URL.</param>
        /// <returns>A <see cref="ShortUrlModel"/> for the specified <paramref name="url"/>.</returns>
        public ShortUrlModel? GetByUrl(string url)
        {
            return QueryDB().Where(x => x.Url == url).FirstOrDefault();
        }

        /// <summary>
        /// Deletes the specified <paramref name="shortUrl"/> from the repository.
        /// </summary>
        /// <param name="shortUrl">The short URL to be deleted.</param>
        public void DeleteByShortUrl(string shortUrl) 
        {
            ExecDB((s) => s.Delete(GetByShortUrl(shortUrl)));
        }
    }
}
