using ComicPoster.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace ComicPoster
{
    public interface ITableService
    {
        CloudTable GetCloudTable();
        ProviderEntity GetProviderEntity(string name);
        void UpdateProviderEntity(ProviderEntity providerEntity, string name, string id);
    }
}