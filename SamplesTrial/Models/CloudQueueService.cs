using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
namespace SamplesTrial.Models
{
    public class CloudQueueService
    {
        public CloudQueue getCloudQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse
                 (ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue sampleQueue = queueClient.GetQueueReference("samplemaker");
            sampleQueue.CreateIfNotExists();

            Trace.TraceInformation("Queue initialized");

            return sampleQueue;
        }
    }
}