using Microsoft.AspNetCore.Mvc;
using System.Net;
using URLShortener.Model;

namespace URLShortener.Controllers
{
    /// <summary>
    /// A controller that handles the REST API for the URL Shortner.
    /// </summary>
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        /// <summary>
        /// Adds a new URL to the repository.
        /// </summary>
        /// <param name="url">The URL being added.</param>
        /// <returns>An instance of the <see cref="IShortUrlModel"/>.</returns>
        [HttpPost]
        [Route("/Add")]
        public IActionResult Add(string url)
        {
            return TryExecuteFunc(() => {
                var model = Program.ShortUrlRepository.Add(url);
                return Ok((ShortUrlDTO)model);
            });
        }

        /// <summary>
        /// Updates a short URL information based on the specified <see cref="ShortUrlDTO"/>.
        /// </summary>
        /// <param name="model">A data transfer object used to update a short URL.</param>
        /// <returns><see cref="OkResult"/> or <see cref="NotFoundResult"/>.</returns>
        [HttpPatch]
        [Route("/Update")]
        public IActionResult Update(ShortUrlDTO model) 
        {
            return TryExecuteFunc(() =>
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                var m = Program.ShortUrlRepository.GetByShortUrl(model.ShortUrl);
                if (m == null)
                {
                    m = Program.ShortUrlRepository.GetById(int.Parse(model.Id));
                    if (m == null)
                        return NotFound();

                    m.ShortUrl = model.ShortUrl;
                }

                m.Url = model.Url;
                Program.ShortUrlRepository.Update(m);

                return Ok((ShortUrlDTO)m);
            });
        }

        /// <summary>
        /// Returns a list with all short URLs.
        /// </summary>
        /// <returns>A list with all short URLs.</returns>
        [HttpGet]
        [Route("/List")]
        public IActionResult List() 
        {
            return TryExecuteFunc(() =>
            {
                var l = Program.ShortUrlRepository.GetAll().Select(x => (ShortUrlDTO)x);
                return Ok(l);
            });
        }

        /// <summary>
        /// Returns the short URL information with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of a short URL.</param>
        /// <returns>The short URL information with the specified <paramref name="id"/>.</returns>
        [HttpGet]
        [Route("/ById/{id}")]
        public IActionResult GetById(int id)
        {
            return TryExecuteFunc(() =>
            {
                var l = Program.ShortUrlRepository.GetById(id);
                if (l == null)
                    return NotFound();

                return Ok((ShortUrlDTO)l);
            });
        }

        /// <summary>
        /// Redirects a short URL to a long URL.
        /// </summary>
        /// <param name="shortUrl">The short URL being redirected.</param>
        [HttpGet]
        [Route("/{shortUrl}")]
        public IActionResult RedirectToLongURL(string shortUrl)
        {
            return TryExecuteFunc(() =>
            {
                shortUrl = SanitizeShortUrl(shortUrl);
                var model = Program.ShortUrlRepository.GetByShortUrl(shortUrl);
                if (model == null)
                {
                    var page404 = Program.App.Configuration.GetValue<string>("404page");
                    if (page404 == null)
                        return NotFound();

                    return Redirect(page404);
                }

                /*
                 * Increase the Hit counter
                 */
                model.Hits += 1;
                Program.ShortUrlRepository.Update(model);

                /*
                 * Redirect to the page
                 */
                return Redirect(model.Url);
            });
        }

        /// <summary>
        /// Deletes a short URL from the repository.
        /// </summary>
        /// <param name="shortUrl">The short URL being deleted.</param>
        [HttpDelete]
        [Route("/{shortUrl}")]
        public IActionResult Delete(string shortUrl)
        {
            return TryExecuteFunc(() =>
            {
                shortUrl = SanitizeShortUrl(shortUrl);
                var model = Program.ShortUrlRepository.GetByShortUrl(shortUrl);
                if (model == null)
                    return NotFound();

                Program.ShortUrlRepository.Delete(model);
                return Ok();
            });
        }

        /// <summary>
        /// Deletes a short URL from the repository.
        /// </summary>
        /// <param name="id">The id of the short URL being deleted.</param>
        [HttpDelete]
        [Route("/ById/{id}")]
        public IActionResult Delete(int id)
        {
            return TryExecuteFunc(() =>
            {
                var model = Program.ShortUrlRepository.GetById(id);
                if (model == null)
                    return NotFound();

                Program.ShortUrlRepository.Delete(model);
                return Ok();
            });
        }

        /// <summary>
        /// Validates a short URL.
        /// </summary>
        /// <param name="shortUrl">The short URL being validated.</param>
        /// <returns>
        /// <see cref="NotFoundResult"/> if the specified short URL does not exit, 
        /// <see cref="OkResult"/> if the long URL can be retrieved, or
        /// <see cref="StatusCodeResult"/> if something went awry.
        /// </returns>
        [HttpGet]
        [Route("/Validate/{shortUrl}")]
        public IActionResult Validate(string shortUrl) 
        {
            return TryExecuteFunc(() =>
            {
                shortUrl = SanitizeShortUrl(shortUrl);
                var m = Program.ShortUrlRepository.GetByShortUrl(shortUrl);
                if (m == null)
                    return NotFound();

                using var cli = new HttpClient();
                var resp = cli.GetAsync(m.Url).GetAwaiter().GetResult();
                if (resp.IsSuccessStatusCode)
                    return Ok();

                return StatusCode((int)resp.StatusCode, resp.StatusCode.ToString());
            });
        }

        /// <summary>
        /// Attempt to execute the specified <see cref="Func{IActionResult}"/>, returning the result of the function if successful or, if an exception occurs, a <see cref="StatusCodeResult"/> with a 500 value.
        /// </summary>
        /// <param name="func">The function to be executed.</param>
        /// <returns>The result of the function if successful or, if an exception occurs, a <see cref="StatusCodeResult"/> with a 500 value.</returns>
        private IActionResult TryExecuteFunc(Func<IActionResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Sanitizes the specified <paramref name="shortUrl"/>.
        /// </summary>
        /// <param name="shortUrl">A short URL to be sanitized.</param>
        /// <returns>A sanitized version of the short URL.</returns>
        private static string SanitizeShortUrl(string shortUrl) 
        {
            return shortUrl.StartsWith(Program.ShortUrlBase, StringComparison.InvariantCultureIgnoreCase) ? shortUrl[Program.ShortUrlBase.Length..] : shortUrl;
        }
    }
}
