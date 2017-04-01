using System.Configuration;

namespace ComicPoster
{
    public static class SettingsHelper
    {
        public static string TableReference => ConfigurationManager.AppSettings["TableReference"];
        public static string Channel => ConfigurationManager.AppSettings["Channel"];
        public static string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"];
        public static string SlackUrl => ConfigurationManager.AppSettings["SlackUrl"];
    }
}