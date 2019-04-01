// Entity class for Azure table
using Microsoft.WindowsAzure.Storage.Table;

namespace ProductStore.Models
{

    public class SampleEntity : TableEntity
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string SampleBlobURL { get; set; }

        public SampleEntity(string partitionKey, string sampleID)
        {
            PartitionKey = partitionKey;
            RowKey = sampleID;
        }

        public SampleEntity() { }

    }
}
