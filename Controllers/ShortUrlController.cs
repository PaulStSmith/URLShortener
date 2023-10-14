using Microsoft.AspNetCore.Mvc;
using URLShortener.Model;

namespace URLShortener.Controllers
{
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        /// <summary>
        /// Adds a new URL to the repository.
        /// </summary>
        /// <param name="url">The URL being added.</param>
        /// <returns>An instance of the <see cref="IShortUrlModel"/>.</returns>
        [HttpPost]
        [Route("/Add/{url}")]
        public IActionResult Add(string url)
        {
            try
            {
                var model = Program.ShortUrlRepository.Add(url);
                return Ok(model);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch]
        [Route("/Update")]
        public IActionResult Update(ShortUrlDTO model) 
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException("model");

                var m = Program.ShortUrlRepository.GetByShortUrl(model.ShortUrl);
                if (m == null)
                    return NotFound();

                m.Url= model.Url;
                Program.ShortUrlRepository.Update(m);

                return Ok(m);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Returns a list with all short URLs.
        /// </summary>
        /// <returns>A list with all short URLs.</returns>
        [HttpGet]
        [Route("/List")]
        public IActionResult List() 
        {
            try
            {
                var l = Program.ShortUrlRepository.GetAll();
                return Ok(l);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Returns the short URL information with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of a short URL.</param>
        /// <returns>The short URL information with the specified <paramref name="id"/>.</returns>
        [HttpGet]
        [Route("/Get/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var l = Program.ShortUrlRepository.GetById(id);
                if (l == null)
                    return NotFound();

                return Ok(l);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Redirects a short URL to a long URL.
        /// </summary>
        /// <param name="shortUrl">The short URL being redirected.</param>
        [HttpGet]
        [Route("/{shortUrl}")]
        public IActionResult RedirectToLongURL(string shortUrl)
        {
            /*
             * TODO: Have a 404 page.
             */
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

        }

        /// <summary>
        /// Deletes a short URL from the repository.
        /// </summary>
        /// <param name="shortUrl">The short URL being deleted.</param>
        [HttpDelete]
        [Route("/{shortUrl}")]
        public IActionResult Delete(string shortUrl)
        {
            try
            {
                var model = Program.ShortUrlRepository.GetByShortUrl(shortUrl);
                if (model == null)
                    return NotFound();

                Program.ShortUrlRepository.Delete(model);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Deletes a short URL from the repository.
        /// </summary>
        /// <param name="shortUrl">The short URL being deleted.</param>
        [HttpDelete]
        [Route("/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var model = Program.ShortUrlRepository.GetById(id);
                if (model == null)
                    return NotFound();

                Program.ShortUrlRepository.Delete(model);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
