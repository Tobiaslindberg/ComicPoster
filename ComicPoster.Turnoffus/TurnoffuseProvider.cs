using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using ComicPoster.Common;

namespace ComicPoster.Turnoffus
{
    public class TurnoffuseProvider : IComicProvider
    {
        private const string ComicUrl = "http://turnoff.us/feed.xml";

        public Comic DownloadComic(string oldId)
        {
            var doc = DownloadXml();
            var items = doc.SelectNodes("/rss/channel/item");
            if (items == null || items.Count == 0)
            {
                return null;
            }

            var item = items[0];

            var guid = item["guid"]?.InnerText;
            var pubDate = item["pubDate"]?.InnerText;
            var title = item["title"]?.InnerText;
            var description = item["description"]?.ChildNodes[0];

            // ReSharper disable PossibleNullReferenceException
            if (guid == null
                || pubDate == null
                || title == null
                || description == null
                || !(description as XmlElement).HasAttribute("alt")
                || !(description as XmlElement).HasAttribute("src")
                || oldId.Equals(pubDate))
            {
                return null;
            }

            var altText = description.Attributes["alt"].InnerText;
            var imageUrl = description.Attributes["src"].InnerText;
            // ReSharper restore PossibleNullReferenceException

            return new Comic
            {
                ComicUrl = new Uri("http://turnoff.us/"),
                PermaLink = new Uri(guid),
                Name = "turnoff.us",
                Id = pubDate,
                Title = title,
                ComicImages = new List<ComicImage>
                {
                    new ComicImage
                    {
                        AltText = altText,
                        ImageUrl = new Uri(imageUrl)
                    }
                }
            };
        }

        private static XmlDocument DownloadXml()
        {
            using (var webClient = new WebClient())
            {
                var xml = webClient.DownloadString(ComicUrl);

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return doc;
            }
        }
    }
}
