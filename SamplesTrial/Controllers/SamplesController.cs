using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SamplesTrial.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;

namespace SamplesTrial.Controllers
{
    public class SamplesController : ApiController
    {
        private const String partitionName = "Samples_Partition_1";
        //setup
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        /// <summary>
        /// Constructor for samples controller
        /// setup
        /// </summary>
        public SamplesController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("Samples");
        }

        /// <summary>
        /// Get all samples
        /// </summary>
        /// <returns>HTTP Response</returns>
        // GET: api/samples
        public IEnumerable<Sample> Get()
        {
            TableQuery<SampleEntity> query = new TableQuery<SampleEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<SampleEntity> entityList = new List<SampleEntity>(table.ExecuteQuery(query));

            // Basically create a list of samples from the list of SampleEntity with a 1:1 object relationship, filtering data as needed
            IEnumerable<Sample> sampleList = from e in entityList
                                              select new Sample()
                                              {
                                                  SampleID = e.RowKey,
                                                  Title = e.Title,
                                                  Artist = e.Artist,
                                                  SampleMp3URL = e.SampleBlobURL,
                                                  CreatedDate = e.CreatedDate,
                                                  Mp3Blob = e.Mp3Blob,
                                                  SampleDate = e.SampleDate,
                                                  SampleMp3Blob = e.SampleMp3Blob

                                              };
            return sampleList;
        }

        // GET: api/samples/5
        /// <summary>
        /// Get a samples
        /// </summary>
        /// <param name="id">sample to get</param>
        /// <returns>HTTP Response</returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult GetSample(string id)
        {
            // Create a retrieve operation that takes a sample entity.
            TableOperation getOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult getOperationResult = table.Execute(getOperation);

            // Construct response including a new DTO as apprporiatte
            if (getOperationResult.Result == null) return NotFound();
            else
            {
                SampleEntity sampleEntity = (SampleEntity)getOperationResult.Result;
                Sample p = new Sample()
                {
                    SampleID = sampleEntity.RowKey,
                    Title = sampleEntity.Title,
                    Artist = sampleEntity.Artist,
                    SampleMp3URL = sampleEntity.SampleBlobURL,
                    CreatedDate = sampleEntity.CreatedDate,
                    Mp3Blob = sampleEntity.Mp3Blob,
                    SampleDate = sampleEntity.SampleDate,
                    SampleMp3Blob = sampleEntity.SampleMp3Blob
                };
                return Ok(p);
            }
        }

        // POST: api/samples
        /// <summary>
        /// Create a new samples
        /// </summary>
        /// <param name="sample">Sample to create</param>
        /// <returns>HTTP Response</returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult PostSample(Sample sample)
        {
            SampleEntity sampleEntity = new SampleEntity()
            {
                RowKey = getNewMaxRowKeyValue(),
                PartitionKey = partitionName,
                Title = sample.Title,
                Artist = sample.Artist,
                CreatedDate = DateTime.Now,
            };

            // Create the TableOperation that inserts the sample entity.
            var insertOperation = TableOperation.Insert(sampleEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { id = sampleEntity.RowKey }, sampleEntity);
        }

        // PUT: api/samples/5
        /// <summary>
        /// Update a samples
        /// </summary>
        /// <param name="id">id to be updated</param>
        /// <param name="sample">new data to update</param>
        /// <returns>HTTP Response</returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSample(string id, Sample sample)
        {
            if (id != sample.SampleID)
            {
                return BadRequest();
            }

            // Create a retrieve operation that takes a samples entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a SampleEntity object.
            SampleEntity updateEntity = (SampleEntity)retrievedResult.Result;


            updateEntity.Title = sample.Title;
            updateEntity.Artist = sample.Artist;


            // Create the TableOperation that inserts the sample entity.
            // Note semantics of InsertOrReplace() which are consistent with PUT
            // See: https://stackoverflow.com/questions/14685907/difference-between-insert-or-merge-entity-and-insert-or-replace-entity
            var updateOperation = TableOperation.InsertOrReplace(updateEntity);

            // Execute the insert operation.
            table.Execute(updateOperation);

            return StatusCode(HttpStatusCode.NoContent);
        }



        //EXPERIMENTAL!!! -0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0

        // accessor variables and methods for blob containers and queues
        private BlobStorageService _blobStorageService = new BlobStorageService();
        private CloudQueueService _queueStorageService = new CloudQueueService();

        /// <summary>
        /// Gets the library containger for songs and samples
        /// </summary>
        /// <returns>CloudBlopContainer - The library container</returns>
        private CloudBlobContainer getLibraryContainer()
        {
            return _blobStorageService.getCloudBlobContainer();
        }

        /// <summary>
        /// Gets the samplemaker queue
        /// </summary>
        /// <returns>CloudQueue - sample maker queue</returns>
        private CloudQueue getSampleMakerQueue()
        {
            return _queueStorageService.getCloudQueue();
        }

        /// <summary>
        /// Gets the MimeType of the specififed file
        /// </summary>
        /// <param name="Filename">String - File name of submited file</param>
        /// <returns>String - Content Type</returns>
        private string GetMimeType(string Filename)
        {
            try
            {
                string ext = Path.GetExtension(Filename).ToLowerInvariant();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (key != null)
                {
                    string contentType = key.GetValue("Content Type") as String;
                    if (!String.IsNullOrEmpty(contentType))
                    {
                        return contentType;
                    }
                }
            }
            catch
            {
            }
            return "application/octet-stream";
        }

        // PUT: api/samples/5/put
        /// <summary>
        /// Updates a existing table entry with a song.
        /// Sample will also be created and added to table entry via webjob
        /// </summary>
        /// <param name="id">ID of entry to be updated</param>
        /// <returns>HTTP Responce</returns>
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/samples/{id}/blob")]
        public async Task<IHttpActionResult> PutBlob(string id)
        {

            //getting our content which is the song to have sample created
            //must be done async so we await it to stop errors
            Stream requestStream = await Request.Content.ReadAsStreamAsync();

            if (requestStream.Length < 1) {
                return  StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            // Create a retrieve operation that takes a sample entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a ProductEntity object.
            SampleEntity updateEntity = (SampleEntity)retrievedResult.Result;
            if (updateEntity == null) {
                return StatusCode(HttpStatusCode.NotFound);
            }

            //Delete any blobs in it that exists
            //reset our values
            DeleteOldBlobs(updateEntity);
            updateEntity.Mp3Blob = "";
            updateEntity.SampleBlobURL = "";
            updateEntity.SampleMp3Blob = "";

            //Set the name of the song to be the id of the table entity
            string name = string.Format("{0}{1}", id, ".mp3");

            //set our full path so its easier
            string path = "songs/" + name;
            var blob = getLibraryContainer().GetBlockBlobReference(path);

            //setting our content type header as audo/mpeg
            blob.Properties.ContentType = Request.Content.Headers.ContentType.ToString();
            //upload song
            blob.UploadFromStream(requestStream);

            //Add to our queue to trigger the webjob
            CloudQueue sampleQueue = getSampleMakerQueue();
            var queueMessageSample = new SampleEntity(partitionName, id);
            sampleQueue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(queueMessageSample)));

            //Update our sample records with the new mp3 blob name
            updateEntity.Mp3Blob = name;

            // Create the TableOperation that inserts the sample entity.
            var updateOperation = TableOperation.InsertOrReplace(updateEntity);

            // Execute the insert operation.
            table.Execute(updateOperation);

            return StatusCode(HttpStatusCode.Created);
        }

        /// <summary>
        /// Deletes a sample blob that is assosiated with a sample
        /// </summary>
        /// <param name="id">Id of the sample to have the blob deleted</param>
        /// <returns>HTTP Response</returns>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("api/samples/{id}/blob")]
        public IHttpActionResult DeleteBlob(string id)
        {

            // Create a retrieve operation that takes a sample entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a ProductEntity object.
            SampleEntity updateEntity = (SampleEntity)retrievedResult.Result;
            if (updateEntity == null)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }

            //Delete any blobs in it that exists
            //reset our values
            DeleteOldBlobs(updateEntity);
            updateEntity.Mp3Blob = "";
            updateEntity.SampleBlobURL = "";
            updateEntity.SampleMp3Blob = "";

            // Create the TableOperation that inserts the sample entity.
            var updateOperation = TableOperation.InsertOrReplace(updateEntity);

            // Execute the insert operation.
            table.Execute(updateOperation);

            return StatusCode(HttpStatusCode.Created);
        }

        //-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0-0


        // DELETE: api/samples/5
        /// <summary>
        /// Delete a samples
        /// </summary>
        /// <param name="id">Sample to be deleted</param>
        /// <returns>HTTP Response</returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult DeleteSample(string id)
        {
            // Create a retrieve operation that takes a samples entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null) return NotFound();
            else
            {
                SampleEntity deleteEntity = (SampleEntity)retrievedResult.Result;
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                DeleteOldBlobs(deleteEntity); //delete blobs assosiated with sample
                table.Execute(deleteOperation);

                return Ok(retrievedResult.Result);
            }
        }

        /// <summary>
        /// Deleted blobs from a samples entity
        /// </summary>
        /// <param name="toDelete">SampleEntity - to have the blob deleted from</param>
        public void DeleteOldBlobs(SampleEntity toDelete) {
            
            //set our paths
            string Songpath = "songs/" + toDelete.Mp3Blob;
            string Samplepath = "audio/samples/" + toDelete.SampleMp3Blob;

            //if there is a mp3blob - delete it
            if (toDelete.Mp3Blob != "") {
                var mp3Blob = getLibraryContainer().GetBlobReference(Songpath);
                mp3Blob.DeleteIfExists();
            }

            //if there is a sample mp3 blob - delete it
            if (toDelete.SampleMp3Blob != "") {
                var sampleblob = getLibraryContainer().GetBlobReference(Samplepath);
                sampleblob.DeleteIfExists();
            }


        }

        private String getNewMaxRowKeyValue()
        {
            TableQuery<SampleEntity> query = new TableQuery<SampleEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));

            int maxRowKeyValue = 0;
            foreach (SampleEntity entity in table.ExecuteQuery(query))
            {
                int entityRowKeyValue = Int32.Parse(entity.RowKey);
                if (entityRowKeyValue > maxRowKeyValue) maxRowKeyValue = entityRowKeyValue;
            }
            maxRowKeyValue++;
            return maxRowKeyValue.ToString();
        }


    }
}
