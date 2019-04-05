using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SamplesTrial.Models;
using System;
using System.Configuration;


namespace SamplesTrial.Migrations
{
    public static class InitialiseSamples
    {
        public static void go()
        {
            const String partitionName = "Samples_Partition_1";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("Samples");

            // If table doesn't already exist in storage then create and populate it with some initial values, otherwise do nothing
            if (!table.Exists())
            {
                // Create table if it doesn't exist already
                table.CreateIfNotExists();

                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                // Create a product entity and add it to the table.
                SampleEntity sample1 = new SampleEntity(partitionName, "1");
                sample1.Title = "Best of Both Worlds";
                sample1.SampleBlobURL = "";
                sample1.Artist = "Hannah  Montannah";
                sample1.CreatedDate = DateTime.Now;
                sample1.Mp3Blob = "Blob Name";
                sample1.SampleDate = DateTime.Now;
                sample1.SampleMp3Blob = "Name of blob in storage";

                SampleEntity sample2 = new SampleEntity(partitionName, "2");
                sample2.Title = "Baby shark";
                sample2.SampleBlobURL = "";
                sample2.Artist = "Youtube";
                sample2.CreatedDate = DateTime.Now;
                sample2.Mp3Blob = "Blob Name";
                sample2.SampleDate = DateTime.Now;
                sample2.SampleMp3Blob = "Name of blob in storage";

                SampleEntity sample3 = new SampleEntity(partitionName, "3");
                sample3.Title = "American Idiot";
                sample3.SampleBlobURL = "";
                sample3.Artist = "Green Day";
                sample3.CreatedDate = DateTime.Now;
                sample3.Mp3Blob = "Blob Name";
                sample3.SampleDate = DateTime.Now;
                sample3.SampleMp3Blob = "Name of blob in storage";

                // Add product entities to the batch insert operation.
                batchOperation.Insert(sample1);
                batchOperation.Insert(sample2);
                batchOperation.Insert(sample3);

                // Execute the batch operation.
                table.ExecuteBatch(batchOperation);
            }

        }
    }
}