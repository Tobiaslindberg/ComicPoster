using System.Collections.Generic;

namespace ComicPoster.Slack
{
    public class Message
    {
        public string Text { get; set; }
        public string Username { get; set; }
        public bool Mrkdwn { get; set; }
        public string Icon_emoji { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string channel { get; set; }
    }
}