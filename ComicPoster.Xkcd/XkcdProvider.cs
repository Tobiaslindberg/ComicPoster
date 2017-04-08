using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using ComicPoster.Common;
using HtmlAgilityPack;

namespace ComicPoster.Xkcd
{
    public class XkcdProvider : IComicProvider
    {
        private const string ComicUrl = "https://xkcd.com/";

        public Comic DownloadComic(string oldId)
        {
            var document = DownloadDocument();

            var htmlNodes = GetHtmlNodes(document);
            var imageUrl = GetImageUrl(htmlNodes);
            var permLink = GetPermLink(htmlNodes);
            var altText = GetAltText(htmlNodes);
            var comicTitle = htmlNodes.FirstOrDefault(x => x.Id.Equals("ctitle", StringComparison.OrdinalIgnoreCase))?.InnerText;

            if (string.IsNullOrEmpty(imageUrl) 
                || string.IsNullOrEmpty(permLink)
                || permLink.Equals(oldId, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var comic = new Comic
            {
                Id = permLink,
                Name = "XKCD",
                ComicUrl = new Uri(ComicUrl),
                PermaLink = new Uri(permLink),
                Title = comicTitle,
                ComicImages = new List<ComicImage>
                {
                    new ComicImage
                    {
                        AltText = HttpUtility.HtmlDecode(altText),
                        ImageUrl = new Uri(imageUrl)
                    }
                }
            };

            return comic;
        }

        private static string GetAltText(IEnumerable<HtmlNode> htmlNodes)
        {
            var comicDiv = htmlNodes.FirstOrDefault(x => x.Id.Equals("comic", StringComparison.OrdinalIgnoreCase));
            var imageDiv = comicDiv?.Descendants().FirstOrDefault(x => x.Name.Equals("img", StringComparison.OrdinalIgnoreCase));

            var altText =
                imageDiv?.Attributes.FirstOrDefault(x => x.Name.Equals("title", StringComparison.OrdinalIgnoreCase))?
                    .Value;

            return altText;
        }

        private string GetPermLink(IEnumerable<HtmlNode> htmlNodes)
        {
            var permLinkNode = htmlNodes.FirstOrDefault(x => x.InnerText.Contains("Permanent link "));
            var permLink = GetLinks(permLinkNode?.InnerText).FirstOrDefault();

            return permLink;
        }

        private string GetImageUrl(IEnumerable<HtmlNode> htmlNodes)
        {
            var imageUrlNode = htmlNodes.FirstOrDefault(x => x.InnerText.Contains("hotlinking/embedding"));
            var imageUrl = GetLinks(imageUrlNode?.InnerText).FirstOrDefault();

            return imageUrl;
        }

        private static List<HtmlNode> GetHtmlNodes(HtmlDocument document)
        {
            var comicBody = document.GetElementbyId("middleContainer");
            var htmlNodes = comicBody.Descendants().ToList();

            return htmlNodes;
        }

        private static HtmlDocument DownloadDocument()
        {
            var webclient = new WebClient();
            var html = webclient.DownloadString(ComicUrl);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            return document;
        }

        private IEnumerable<string> GetLinks(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            var urlRegex = new Regex(
                @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*",
                RegexOptions.IgnoreCase);
            var matches = urlRegex.Matches(message);

            return (from Match match in matches select match.Value).ToList();
        }
    }
}
