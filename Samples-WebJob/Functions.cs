using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;
using Microsoft.WindowsAzure.Storage.Table;
using ProductStore.Models;

namespace Samples_WebJob
{
    /// <summary>
    /// Daryl McAllister
    /// S1222204
    /// Cloud Platform Development Coursework
    /// 
    /// References: 
    /// Lab 2, Cloud Platform Development, Glasgow Caledonian University, 2019
    /// Class1.cs sample code, Cloud Platform Development, Glasgow Caledonian University, 2019
    /// CPD diet 1 CWK part A resources, Cloud Platform Development, Glasgow Caledonian University, 2019
    /// </summary>
    public class Functions
    {

        /// <summary>
        /// This class contains the application-specific WebJob code consisting of event-driven
        /// methods executed when messages appear in queues with any supporting code.
        /// 
        /// Trigger method  - run when new message detected in queue. "samplemaker" is name of queue.
        /// "library" is name of storage container; "songs" and "samples" are folder names. 
        /// "{queueTrigger}" is an inbuilt variable taking on value of contents of message automatically;
        /// the other variables are valued automatically.
        /// </summary>
        /// <param name="blobInfo">String - inbuilt variable taking on value of contents of message automatically</param>
        /// <param name="inputBlob">CloudBlockBlob - The input song that will have a sample generated</param>
        /// <param name="outputBlob">CloudBlockBlob - The output sammple</param>
        /// <param name="logger">TextWriter - Logger for writeing information states to trace output </param>
        public static void GenerateSample(
        [QueueTrigger("audiosamplemaker")] SampleEntity sampleInQueue,
        [Table("Samples", "{PartitionKey}", "{RowKey}")] SampleEntity sampleInTable,
        [Table("Samples")] CloudTable tableBinding, TextWriter logger)
        {
            //use log.WriteLine() rather than Console.WriteLine() for trace output
            logger.WriteLine("GenerateSample() started...");
            logger.WriteLine("Input blob is: " + sampleInQueue.Title);


            BlobStorageService BlobStorage = new BlobStorageService(); ;
            CloudBlobContainer blobContainer = BlobStorage.getCloudBlobContainer();


            var inputblob = blobContainer.GetDirectoryReference("audio/full/")
                .GetBlobReference(sampleInTable.Mp3Blob);


            string sampleName = string.Format("{0}-{1}{2}", Guid.NewGuid(), sampleInTable.Title, ".mp3");
            sampleInTable.SampleMp3Blob = sampleName;


            var outputblob = blobContainer.GetBlockBlobReference("audio/samples/" + sampleName);


            // open streams to blobs for reading and writing as appropriate.
            // pass references to application specific methods
            using (Stream input = inputblob.OpenRead())
            using (Stream output = outputblob.OpenWrite())
            {
                CreateSample(input, output, 20);
                outputblob.Properties.ContentType = "audio/mpeg3";
            }

            // Update sample date
            sampleInTable.SampleDate = DateTime.Now;

            // Creates and executes an update operation to update the entity in the table
            TableOperation updateOperation = TableOperation.InsertOrReplace(sampleInTable);
            tableBinding.Execute(updateOperation);

            logger.WriteLine("GenerageSample() completed...");
        }

        /// <summary>
        /// Function takes the stream input of audio and creates a sample clip with the specified duration
        /// outputs into the stream output
        /// </summary>
        /// <param name="input">Stream - input audio song that will have a sample generated</param>
        /// <param name="output">Stream - output, where the sample will be stored</param>
        /// <param name="duration">int - the duration that the sample should last for</param>
        private static void CreateSample(Stream input, Stream output, int duration)
        {
            using (var reader = new Mp3FileReader(input, wave => new NLayer.NAudioSupport.Mp3FrameDecompressor(wave)))
            {
                Mp3Frame frame;
                frame = reader.ReadNextFrame();
                int frameTimeLength = (int)(frame.SampleCount / (double)frame.SampleRate * 1000.0);
                int framesRequired = (int)(duration / (double)frameTimeLength * 1000.0);

                int frameNumber = 0;
                while ((frame = reader.ReadNextFrame()) != null)
                {
                    frameNumber++;

                    if (frameNumber <= framesRequired)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                    else break;
                }
            }
        }
    }
}
