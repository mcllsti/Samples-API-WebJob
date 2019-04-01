// This is a Data Transfer Object (DTO) class. This is sent/received in REST requests/responses.
// Read about DTOS here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5

using System.ComponentModel.DataAnnotations;

namespace ProductStore.Models
{
    public class Sample
    {
        /// <summary>
        /// Product ID
        /// </summary>
        [Key]
        public string SampleID { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Category of the product
        /// </summary>
        public string SampleBlobURL { get; set; }
    }
}