using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ComicPoster.Azure
{
    public class CloudStorageFactory
    {
        private readonly string _storageConnectionString;

        public CloudStorageFactory()
        {
            _storageConnectionString = SettingsHelper.StorageConnectionString;
        }

        public CloudStorageAccount Create()
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            return storageAccount;
        }

        public CloudTableClient CreateAndOpen()
        {
            var connection = Create();
            return connection.CreateCloudTableClient();
        }
    }
}