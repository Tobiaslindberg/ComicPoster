using ComicPoster.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace ComicPoster
{
    public class CloudTableService : ITableService
    {
        private const string PartitionKey = "ComicPoster";
        private CloudTable _table;

        public CloudTable GetCloudTable()
        {
            if (_table != null)
            {
                return _table;
            }

            var cloudStorageFactory = new CloudStorageFactory();
            var cloudTableClient = cloudStorageFactory.CreateAndOpen();

            var table = cloudTableClient.GetTableReference(SettingsHelper.TableReference);
            table.CreateIfNotExists();

            return _table = table;
        }

        public ProviderEntity GetProviderEntity(string name)
        {
            var retrieveOperation = TableOperation.Retrieve<ProviderEntity>(PartitionKey, name);
            var retrievedResult = GetCloudTable().Execute(retrieveOperation);
            var providerEntity = (ProviderEntity) retrievedResult.Result;

            return providerEntity;
        }

        public void UpdateProviderEntity(ProviderEntity providerEntity, string name, string id)
        {
            if (providerEntity == null)
            {
                providerEntity = new ProviderEntity(name)
                {
                    LastId = id
                };
            }
            else
            {
                providerEntity.LastId = id;
            }

            var insertOrReplaceOperation = TableOperation.InsertOrReplace(providerEntity);
            GetCloudTable().Execute(insertOrReplaceOperation);
        }
    }
}