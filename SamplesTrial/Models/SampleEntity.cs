using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SamplesTrial.Models
{
    /// <summary>
    /// SampleEntity class acts as the DTO for API when dealing with samples and serlization
    /// </summary>
    public class SampleEntity : TableEntity
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string SampleBlobURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Mp3Blob { get; set; }
        public string SampleMp3Blob { get; set; }
        public DateTime SampleDate { get; set; }

        public SampleEntity(string partitionKey, string sampleID)
        {
            PartitionKey = partitionKey;
            RowKey = sampleID;
        }

        public SampleEntity() { }

    }
}
