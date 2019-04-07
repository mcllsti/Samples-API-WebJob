using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SamplesTrial.Models;
using System;
using System.Configuration;


namespace SamplesTrial.Migrations
{
    /// <summary>
    /// Data migration class populating the table
    /// </summary>
    public static class InitialiseSamples
    {
        /// <summary>
        /// Populate table
        /// </summary>
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
                sample1.Mp3Blob = "";
                sample1.SampleDate = DateTime.Now;
                sample1.SampleMp3Blob = "";

                SampleEntity sample2 = new SampleEntity(partitionName, "2");
                sample2.Title = "Baby shark";
                sample2.SampleBlobURL = "";
                sample2.Artist = "Youtube";
                sample2.CreatedDate = DateTime.Now;
                sample2.Mp3Blob = "";
                sample2.SampleDate = DateTime.Now;
                sample2.SampleMp3Blob = "";

                SampleEntity sample3 = new SampleEntity(partitionName, "3");
                sample3.Title = "American Idiot";
                sample3.SampleBlobURL = "";
                sample3.Artist = "Green Day";
                sample3.CreatedDate = DateTime.Now;
                sample3.Mp3Blob = "";
                sample3.SampleDate = DateTime.Now;
                sample3.SampleMp3Blob = "";

                // Create a product entity and add it to the table.
                SampleEntity sample4 = new SampleEntity(partitionName, "4");
                sample4.Title = "USA Anthem";
                sample4.SampleBlobURL = "";
                sample4.Artist = "USA";
                sample4.CreatedDate = DateTime.Now;
                sample4.Mp3Blob = "";
                sample4.SampleDate = DateTime.Now;
                sample4.SampleMp3Blob = "";

                SampleEntity sample5 = new SampleEntity(partitionName, "5");
                sample5.Title = "French Anthem";
                sample5.SampleBlobURL = "";
                sample5.Artist = "France";
                sample5.CreatedDate = DateTime.Now;
                sample5.Mp3Blob = "";
                sample5.SampleDate = DateTime.Now;
                sample5.SampleMp3Blob = "";

                SampleEntity sample6 = new SampleEntity(partitionName, "6");
                sample6.Title = "Russian Anthem";
                sample6.SampleBlobURL = "";
                sample6.Artist = "Russia";
                sample6.CreatedDate = DateTime.Now;
                sample6.Mp3Blob = "";
                sample6.SampleDate = DateTime.Now;
                sample6.SampleMp3Blob = "";

                // Add product entities to the batch insert operation.
                batchOperation.Insert(sample1);
                batchOperation.Insert(sample2);
                batchOperation.Insert(sample3);
                batchOperation.Insert(sample4);
                batchOperation.Insert(sample5);
                batchOperation.Insert(sample6);

                // Execute the batch operation.
                table.ExecuteBatch(batchOperation);
            }

        }
    }
}