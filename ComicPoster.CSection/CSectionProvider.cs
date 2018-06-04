using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ComicPoster.Common;
using HtmlAgilityPack;

namespace ComicPoster.CSection
{
    public class CSectionProvider : IComicProvider
    {
        private const string ComicUrl = "https://www.csectioncomics.com";

        public Comic DownloadComic(string oldId)
        {
            var document = DownloadDocument();

            var comicBody = document.GetElementbyId("comic");

            var imageNode = comicBody.Descendants().FirstOrDefault(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));
            var comicUrlNode = document.GetElementbyId("permalink");

            var imageUrl = imageNode?.Attributes
                .FirstOrDefault(x => x.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            var imageAlt = imageNode?.Attributes
                .FirstOrDefault(x => x.Name.Equals("alt", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            var comicUrl = comicUrlNode?.Attributes
                .FirstOrDefault(x => x.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            if (comicUrl == null
                || comicUrl.Equals(oldId, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            var comicTitle = comicUrl?.Replace(ComicUrl, "").Replace("/comics/", "").Replace("-", " ");

            var bonusComicUrl = string.Empty;
            var bonusComicBody = document.GetElementbyId("bonus-comic");

            if (bonusComicBody != null)
            {
                var bonusComicImageNode = bonusComicBody.Descendants()
                    .FirstOrDefault(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));

                bonusComicUrl = bonusComicImageNode?.Attributes
                    .FirstOrDefault(x => x.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                    ?.Value;
            }

            if (string.IsNullOrEmpty(comicUrl)
                || string.IsNullOrEmpty(imageUrl)
                || string.IsNullOrEmpty(comicTitle))
            {
                return null;
            }

            var comicImages = new List<ComicImage>
            {
                new ComicImage
                {
                    AltText = imageAlt,
                    ImageUrl = new Uri(imageUrl)
                }
            };

            if (!string.IsNullOrEmpty(bonusComicUrl))
            {
                comicImages.Add(new ComicImage
                {
                    AltText = "Bonus panel",
                    ImageUrl = new Uri(bonusComicUrl)
                });
            }

            var comic = new Comic
            {
                Name = "C-Section",
                PermaLink = new Uri(comicUrl),
                ComicUrl = new Uri(ComicUrl),
                Id = comicUrl,
                Title = comicTitle,
                ComicImages = comicImages
            };

            return comic;
        }

        private static HtmlDocument DownloadDocument()
        {
            var webclient = new WebClient();
            webclient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            var html = webclient.DownloadString(ComicUrl);

            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
    }
}
