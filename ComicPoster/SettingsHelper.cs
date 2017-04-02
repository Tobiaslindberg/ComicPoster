using System.Configuration;

namespace ComicPoster
{
    public static class SettingsHelper
    {
        public static string TableReference => ConfigurationManager.AppSettings["TableReference"];
        public static string Channel => ConfigurationManager.AppSettings["Channel"];
        public static string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"];
        public static string SlackUrl => ConfigurationManager.AppSettings["SlackUrl"];

        public static bool UseTableService
        {
            get
            {
                bool result;
                return bool.TryParse(ConfigurationManager.AppSettings["UseTableService"], out result) && result;
            }
        }
    }
}