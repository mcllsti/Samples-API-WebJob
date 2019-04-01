using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ProductStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProductStore.Controllers
{
    public class SamplesController : ApiController
    {
        private const String partitionName = "Samples_Partition_1";

        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public SamplesController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("Samples");
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        public IEnumerable<Sample> Get()
        {
            TableQuery<SampleEntity> query = new TableQuery<SampleEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<SampleEntity> entityList = new List<SampleEntity>(table.ExecuteQuery(query));

            // Basically create a list of Product from the list of ProductEntity with a 1:1 object relationship, filtering data as needed
            IEnumerable<Sample> productList = from e in entityList
                                               select new Sample()
                                               {
                                                   SampleID = e.RowKey,
                                                   Title = e.Title,
                                                   Artist = e.Artist,
                                                   SampleBlobURL = e.SampleBlobURL
                                               };
            return productList;
        }

        // GET: api/Products/5
        /// <summary>
        /// Get a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult GetSample(string id)
        {
            // Create a retrieve operation that takes a product entity.
            TableOperation getOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult getOperationResult = table.Execute(getOperation);

            // Construct response including a new DTO as apprporiatte
            if (getOperationResult.Result == null) return NotFound();
            else
            {
                SampleEntity productEntity = (SampleEntity)getOperationResult.Result;
                Sample p = new Sample()
                {
                    SampleID = productEntity.RowKey,
                    Title = productEntity.Title,
                    Artist = productEntity.Artist,
                    SampleBlobURL = productEntity.SampleBlobURL
                };
                return Ok(p);
            }
        }

        // POST: api/Products
        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult PostSample(Sample product)
        {
            SampleEntity productEntity = new SampleEntity()
            {
                RowKey = getNewMaxRowKeyValue(),
                PartitionKey = partitionName,
                Title = product.Title,
                Artist = product.Artist,
                SampleBlobURL = product.SampleBlobURL
            };

            // Create the TableOperation that inserts the product entity.
            var insertOperation = TableOperation.Insert(productEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { id = productEntity.RowKey }, productEntity);
        }

        // PUT: api/Products/5
        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSample(string id, Sample product)
        {
            if (id != product.SampleID)
            {
                return BadRequest();
            }

            // Create a retrieve operation that takes a product entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a ProductEntity object.
            SampleEntity updateEntity = (SampleEntity)retrievedResult.Result;

            updateEntity.Title = product.Title;
            updateEntity.Artist = product.Artist;
            updateEntity.SampleBlobURL = product.SampleBlobURL;

            // Create the TableOperation that inserts the product entity.
            // Note semantics of InsertOrReplace() which are consistent with PUT
            // See: https://stackoverflow.com/questions/14685907/difference-between-insert-or-merge-entity-and-insert-or-replace-entity
            var updateOperation = TableOperation.InsertOrReplace(updateEntity);

            // Execute the insert operation.
            table.Execute(updateOperation);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Products/5
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Sample))]
        public IHttpActionResult DeleteSample(string id)
        {
            // Create a retrieve operation that takes a product entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<SampleEntity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null) return NotFound();
            else
            {
                SampleEntity deleteEntity = (SampleEntity)retrievedResult.Result;
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                table.Execute(deleteOperation);

                return Ok(retrievedResult.Result);
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
