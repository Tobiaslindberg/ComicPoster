using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ComicPoster.Common;
using HtmlAgilityPack;

namespace ComicPoster.Dilbert
{
    public class DilbertProvider : IComicProvider
    {
        private const string ComicUrl = "http://dilbert.com/";

        public Comic DownloadComic(string oldId)
        {
            var document = DownloadDocument();

            var comicBody = document.GetElementbyId("js_comics");

            var comicDiv =
                comicBody.Descendants()
                    .FirstOrDefault(
                        x =>
                            x.Attributes.Any(
                                y => y.Value.Equals("img-comic-container", StringComparison.OrdinalIgnoreCase)));

            var comicTitle = comicBody.Descendants()
                .FirstOrDefault(
                    x => x.Attributes.Any(y => y.Value.Equals("comic-title-name", StringComparison.OrdinalIgnoreCase)))?
                .InnerText;

            var imageContainter = comicDiv?.ChildNodes
                .FirstOrDefault(x => x.Name.Equals("a", StringComparison.OrdinalIgnoreCase));
            var imageNode = imageContainter?.ChildNodes
                .FirstOrDefault(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));

            var imageUrl =imageNode?.Attributes
                .FirstOrDefault(x => x.Name.Equals("src", StringComparison.OrdinalIgnoreCase))?
                    .Value;
            var comicUrl = imageContainter?.Attributes
                .FirstOrDefault(
                    x => x.Name.Equals("href", StringComparison.OrdinalIgnoreCase))?.Value;

            if (oldId.Equals(comicUrl, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(comicTitle)
                || string.IsNullOrEmpty(imageUrl)
                || string.IsNullOrEmpty(comicUrl))
            {
                return null;
            }

            var comic = new Comic
            {
                Name = "Dilbert",
                PermaLink = new Uri(comicUrl),
                ComicUrl = new Uri(ComicUrl),
                Id = comicUrl,
                Title = HttpUtility.HtmlDecode(comicTitle),
                ComicImages = new List<ComicImage>
                {
                    new ComicImage
                    {
                        AltText = null,
                        ImageUrl = new Uri(imageUrl)
                    }
                }
            };

            return comic;
        }

        private static HtmlDocument DownloadDocument()
        {
            var webclient = new WebClient();
            var html = webclient.DownloadString(ComicUrl);

            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
    }
}