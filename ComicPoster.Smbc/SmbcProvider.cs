using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ComicPoster.Common;
using HtmlAgilityPack;

namespace ComicPoster.Smbc
{
    public class SmbcProvider : IComicProvider
    {
        private const string ComicUrl = "http://www.smbc-comics.com";

        public Comic DownloadComic(string oldId)
        {
            var document = DownloadDocument();

            var permaLinkUri = GetPermaLink(document);

            if (permaLinkUri == null
                || oldId.Equals(permaLinkUri.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var images = GetImageNodes(document);
            var title = GetTitle(permaLinkUri);

            var comic = new Comic
            {
                Id = permaLinkUri.ToString(),
                ComicUrl = new Uri(ComicUrl),
                Name = "Saturday Morning Breakfast Cereal",
                Title = title,
                PermaLink = permaLinkUri,
                ComicImages = new List<ComicImage>()
            };

            foreach (var htmlNode in images)
            {
                var image = CreateImage(htmlNode);

                if (image == null)
                {
                    continue;
                }

                comic.ComicImages.Add(image);
            }

            return !comic.ComicImages.Any() ? null : comic;
        }

        private static ComicImage CreateImage(HtmlNode htmlNode)
        {
            var src =
                htmlNode.Attributes.FirstOrDefault(x => x.Name.Equals("src", StringComparison.OrdinalIgnoreCase))?
                    .Value;
            var altText =
                htmlNode.Attributes.FirstOrDefault(x => x.Name.Equals("title", StringComparison.OrdinalIgnoreCase))?
                    .Value;

            if (string.IsNullOrEmpty(src))
            {
                return null;
            }

            return new ComicImage
            {
                AltText = HttpUtility.HtmlDecode(altText),
                ImageUrl = new Uri(src)
            };
        }

        private static HtmlDocument DownloadDocument()
        {
            using (var webClient = new WebClient())
            {
                var html = webClient.DownloadString(ComicUrl);

                var document = new HtmlDocument();
                document.LoadHtml(html);
                return document;
            }
        }

        private static string GetTitle(Uri permaLinkUri)
        {
            var title = permaLinkUri.ToString().Split('/').Last();
            if (string.IsNullOrEmpty(title))
            {
                title = "..";
            }
            else
            {
                title = title.Replace("-", " ");
                title = title.First().ToString().ToUpper() + title.Substring(1);
            }

            return title;
        }

        private static Uri GetPermaLink(HtmlDocument document)
        {
            var permaLinkText = document.GetElementbyId("permalinktext");
            var permaLink = permaLinkText.Attributes
                .FirstOrDefault(x => x.Name.Equals("value", StringComparison.OrdinalIgnoreCase))?.Value;

            var permaLinkUri = permaLink != null ? new Uri(permaLink) : null;

            return permaLinkUri;
        }

        private static IEnumerable<HtmlNode> GetImageNodes(HtmlDocument document)
        {
            var comicBody = document.GetElementbyId("cc-comicbody");

            var images = comicBody.Descendants()
                .Where(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));

            return images;
        }
    }
}