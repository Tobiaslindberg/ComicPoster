using System;
using ComicPoster.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace ComicPoster
{
    public class NullTableService : ITableService
    {
        public CloudTable GetCloudTable()
        {
            return new CloudTable(new Uri(""));
        }

        public ProviderEntity GetProviderEntity(string name)
        {
            return new ProviderEntity
            {
                LastId = string.Empty
            };
        }

        public void DeleteAllProviders()
        {
        }

        public void UpdateProviderEntity(ProviderEntity providerEntity, string name, string id)
        {
        }
    }
}