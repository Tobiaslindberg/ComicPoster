using System.Collections.Generic;

namespace ComicPoster.Slack
{
    public class Attachment
    {
        public string Fallback { get; set; }
        public string color { get; set; }
        public string pretext { get; set; }
        public string author_name { get; set; }
        public string author_link { get; set; }
        public string author_icon { get; set; }
        public string title { get; set; }
        public string title_link { get; set; }
        public string text { get; set; }
        public List<Field> fields { get; set; }
        public string image_url { get; set; }
        public string thumb_url { get; set; }
    }
}