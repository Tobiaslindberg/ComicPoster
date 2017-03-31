using Microsoft.WindowsAzure.Storage.Table;

namespace ComicPoster.Azure
{
    public class ProviderEntity : TableEntity
    {
        public ProviderEntity(string providerName)
        {
            PartitionKey = "ComicPoster";
            RowKey = providerName;
        }

        public ProviderEntity() { }

        public string LastId { get; set; }
    }
}