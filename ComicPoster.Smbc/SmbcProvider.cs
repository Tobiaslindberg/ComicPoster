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
            var webclient = new WebClient();
            
            var html = webclient.DownloadString(ComicUrl);
            
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var comicBody = document.GetElementbyId("cc-comicbody");
            var images = comicBody.Descendants()
                .Where(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));
            var permaLinkText = document.GetElementbyId("permalinktext");
            var permaLink = permaLinkText.Attributes.FirstOrDefault(x => x.Name.Equals("value", StringComparison.OrdinalIgnoreCase))?.Value;
            var permaLinkUri = permaLink != null ? new Uri(permaLink) : null;

            if (permaLinkUri == null 
                || oldId.Equals(permaLinkUri?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var title = permaLinkUri.ToString().Split('/').Last();
            if (string.IsNullOrEmpty(title))
            {
                title = "..";
            }

            var comic = new Comic
            {
                Id = permaLinkUri.ToString(),
                ComicUrl = new Uri(ComicUrl),
                Name = "Saturday Morning Breakfast Cereal",
                Title = title,
                PermaLink = permaLinkUri,
                ComicImages = new List<ComicImage>(),
            };

            foreach (var htmlNode in images)
            {
                var src = htmlNode.Attributes.FirstOrDefault(x => x.Name.Equals("src", StringComparison.OrdinalIgnoreCase))?.Value;
                var altText = htmlNode.Attributes.FirstOrDefault(x => x.Name.Equals("title", StringComparison.OrdinalIgnoreCase))?.Value;

                if (string.IsNullOrEmpty(src))
                {
                    continue;
                }

                comic.ComicImages.Add(new ComicImage
                {
                    AltText = HttpUtility.HtmlDecode(altText),
                    ImageUrl = new Uri(src)
                });
            }

            return !comic.ComicImages.Any() ? null : comic;
        }
    }
}
