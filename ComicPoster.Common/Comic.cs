using System;
using System.Collections.Generic;

namespace ComicPoster.Common
{
    public class Comic
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public Uri PermaLink { get; set; }
        public List<ComicImage> ComicImages { get; set; }
        public Uri ComicUrl { get; set; }
    }
}